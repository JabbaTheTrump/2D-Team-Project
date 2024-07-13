using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class PlayerInputController : NetworkBehaviour
{
    [SerializeField] PlayerMovementHandler _movementHandler;
    [SerializeField] InteractionHandler _interactionHandler;

    InventorySystem _inventory;

    //Input Actions
    [HideInInspector] PlayerInput _playerInput;
    [HideInInspector] public InputAction SprintAction;
    [HideInInspector] public InputAction WalkAction;

    private void Start()
    {
        _inventory = GetComponentInChildren<InventorySystem>();

        if (IsLocalPlayer)
        {
            StartCoroutine(LocalPlayerSpawnController.Instance.InvokeLocalPlayerEventNextFrame(gameObject));
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        enabled = IsOwner;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _movementHandler.moveDir = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                _movementHandler.StartSprinting(); 
                break;
            case InputActionPhase.Canceled: 
                _movementHandler.StopSprinting(); 
                break;
        }
    }

    public void OnDrop()
    {
        DropItemServerRpc();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        switch (context.interaction)
        {
            case TapInteraction:
                if (context.phase == InputActionPhase.Performed) _interactionHandler.OnInteractPress();
            break;

            case HoldInteraction:
                switch (context.phase)
                {
                    case InputActionPhase.Performed:
                        _interactionHandler.OnInteractHold();
                        break;

                    case InputActionPhase.Canceled:

                        break;
                }

            break;
        }
    }


    [ServerRpc(RequireOwnership = false)]
    void DropItemServerRpc()
    {
        _inventory.TryRemoveItem(_inventory.SelectedSlot, true);
    }
}
