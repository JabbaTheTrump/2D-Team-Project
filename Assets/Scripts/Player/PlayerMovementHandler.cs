using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public enum MovementState
{
    Idle,
    Walking,
    Sprinting
}

public class PlayerMovementHandler : ServerSideNetworkedBehaviour
{
    [Header("Parameters")]
    [SerializeField] float _walkSpeed = 300;
    [SerializeField] float _sprintSpeed = 500;

    [field: SerializeField] public float MaxStamina { get; private set; } = 100;
    [field: SerializeField] public float CurrentStamina { get; private set; } = 100;
    [field: SerializeField] public float StaminaDrainRate { get; private set; } = 20;
    [field: SerializeField] public float StaminaRecoveryRate { get; private set; } = 15;
    [field: SerializeField] public float StaminaSprintThreshold { get; private set; } = 20; //The minimum stamina required to start a sprint
    [field: SerializeField] public float RecoveryPeriod { get; private set; } = 2;

    [Header("Serialized References")]
    [SerializeField] Rigidbody2D _rb2d;
    [SerializeField] PlayerInputController _inputController;

    [Header("Debug")]
    public ObservableVariable<MovementState> CurrentMovementState = new(MovementState.Idle);

    public bool IsRecoveringFromSprint = false;

    //unserialized fields
    [HideInInspector] public Vector2 moveDir;

    //Events
    public event Action<float> OnStaminaChanged;

    public override void OnNetworkSpawn()
    {
        enabled = IsOwner;
    }

    public void SetActions() //Gets called by the input controller to sub to it's actions once they are set
    {
        _inputController.SprintAction.started += _ => StartSprinting();
        _inputController.SprintAction.canceled += _ => StopSprinting();
    }

    private void FixedUpdate()
    {
        float moveSpeed = _walkSpeed;

        if (moveDir == Vector2.zero) CurrentMovementState.Value = MovementState.Idle; //If the player isn't pressing the WASD keys, consider him idle
        else if (CurrentMovementState.Value != MovementState.Sprinting) CurrentMovementState.Value = MovementState.Walking; //If the player is pressing the WASD keys but isn't running, consider him walking
        else //If the player is sprinting 
        {
            if (0 >= CurrentStamina)
            {
                StopSprinting();
            }
            else
            {
                moveSpeed = _sprintSpeed;
            }
        }

        UpdateStamina();
        _rb2d.velocity = Time.deltaTime * moveSpeed * moveDir;
    }

    void UpdateStamina()
    {
        float previousStamina = CurrentStamina;

        if (CurrentMovementState.Value == MovementState.Sprinting) //Drains stamina if the player is sprinting
        {
            CurrentStamina -= StaminaDrainRate * Time.deltaTime; //Reduce stamina

            if (0 > CurrentStamina) CurrentStamina = 0; //Set it to zero if it is negative
        }
        else if (MaxStamina > CurrentStamina && !IsRecoveringFromSprint) //Recovers stamina if the player isn't sprinting 
        {
            CurrentStamina += StaminaRecoveryRate * Time.deltaTime;
            if (CurrentStamina > MaxStamina) CurrentStamina = MaxStamina;
        }

        if (previousStamina != CurrentStamina)
        {
            OnStaminaChanged?.Invoke(CurrentStamina);
        }
    }

    void StartSprinting()
    {
        if (StaminaSprintThreshold > CurrentStamina) return; //Returns if there isn't enough stamina to start a sprint
        CurrentMovementState.Value = MovementState.Sprinting;
        //Debug.Log("Started sprinting");
    }

    void StopSprinting()
    {
        if (CurrentMovementState.Value != MovementState.Sprinting) return; //Return if the player isn't currently sprinting
        CurrentMovementState.Value = MovementState.Idle;

        //Debug.Log("Stopped sprinting");

        StartCoroutine(StartSprintRecoveryPeriod());
    }

    IEnumerator StartSprintRecoveryPeriod() //Stops the player from recovering stamina for a duration after a sprint is stopped
    {
        IsRecoveringFromSprint = true;

        yield return new WaitForSeconds(RecoveryPeriod);

        IsRecoveringFromSprint = false;
    }
}
