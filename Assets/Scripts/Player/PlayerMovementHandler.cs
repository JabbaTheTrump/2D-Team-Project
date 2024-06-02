using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementHandler : NetworkBehaviour
{
    [Header("Parameters")]
    [SerializeField] float _walkSpeed = 300;
    [SerializeField] float _sprintSpeed = 500;

    [field: SerializeField] public float MaxStamina { get; private set; } = 100;
    [field: SerializeField] public float CurrentStamina { get; private set; } = 100;
    [field: SerializeField] public float StaminaDrainRate { get; private set; } = 20;
    [field: SerializeField] public float StaminaRecoveryRate { get; private set; } = 15;
    [field: SerializeField] public float StaminaSprintThreshold { get; private set; } = 20; //The minimum stamina required to start a sprint

    [Header("Serialized References")]
    [SerializeField] Rigidbody2D _rb2d;
    [SerializeField] PlayerInputController _inputController;

    [Header("Debug")]
    public bool IsRunning = false;

    //unserialized fields
    [HideInInspector] public Vector2 moveDir;

    //Events
    public event Action<bool> OnSprintStateChanged;

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

        if (IsRunning)
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
        if (IsRunning)
        {
            CurrentStamina -= StaminaDrainRate * Time.deltaTime;
            if (0 > CurrentStamina) CurrentStamina = 0;
        }
        else if (MaxStamina > CurrentStamina)
        {
            CurrentStamina += StaminaRecoveryRate * Time.deltaTime;
            if (CurrentStamina > MaxStamina) CurrentStamina = MaxStamina;
        }
    }

    void StartSprinting()
    {
        if (StaminaSprintThreshold > CurrentStamina) return; //Returns if there isn't enough stamina to start a sprint

        Debug.Log("Started sprinting");
        IsRunning = true;
        OnSprintStateChanged?.Invoke(IsRunning);
    }

    void StopSprinting()
    {
        if (IsRunning)
        {
            Debug.Log("Stopped sprinting");
            IsRunning = false;
            OnSprintStateChanged?.Invoke(IsRunning);
        }
    }
}
