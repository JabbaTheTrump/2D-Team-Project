using System;
using System.Collections;
using Unity.Tutorials.Core.Editor;
using UnityEngine;



public class PlayerMovementHandler : MovementHandler
{
    [field: SerializeField] public float MaxStamina { get; private set; } = 100;
    [field: SerializeField] public float CurrentStamina { get; private set; } = 100;
    [field: SerializeField] public float StaminaDrainRate { get; private set; } = 20;
    [field: SerializeField] public float StaminaRecoveryRate { get; private set; } = 15;
    [field: SerializeField] public float StaminaSprintThreshold { get; private set; } = 20; //The minimum stamina required to start a sprint
    [field: SerializeField] public float RecoveryPeriod { get; private set; } = 2;

    [Space]
    [Header("Player Noise")]
    [SerializeField] float _walkNoiseRadius = 2;
    [SerializeField] float _sprintNoiseRadius = 8;

    [Header("Serialized References")]
    [SerializeField] Rigidbody2D _rb2d;
    [SerializeField] PlayerInputController _inputController;

    [Header("Debug")]
    public bool IsRecoveringFromSprint = false;

    //unserialized fields
    [HideInInspector] public Vector2 moveDir;

    //Events
    public event Action<float> OnStaminaChanged;

    public void SetActions() //Gets called by the input controller to sub to it's actions once they are set
    {
        _inputController.SprintAction.started += _ => StartSprinting();
        _inputController.SprintAction.canceled += _ => StopSprinting();
    }

    private void FixedUpdate()
    {
        if (moveDir == Vector2.zero)
        {
            CurrentMovementState.Value = MovementState.Idle; //If the player isn't pressing the WASD keys, consider him idle
        }

        else if (CurrentMovementState.Value != MovementState.Sprinting)
        {
            CurrentMovementState.Value = MovementState.Walking; //If the player is pressing the WASD keys but isn't running, consider him walking

            AINoiseManager.Instance.CreateNoiseAtPoint(transform.position, _walkNoiseRadius);
        }

        else if (0 >= CurrentStamina) StopSprinting(); //If the player is sprinting and out of stamina

        else //If the player is sprinting
        {
            AINoiseManager.Instance.CreateNoiseAtPoint(transform.position, _sprintNoiseRadius);
        }

        UpdateStamina();
        _rb2d.velocity = Time.deltaTime * GetMovementTypeByState(CurrentMovementState.Value).Velocity * moveDir;
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
        if (previousStamina != CurrentStamina) //Invoke an event if the stamina changes **TRY TO IMPELEMENT OBSERVABLE VARIABLE INSTEAD
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
