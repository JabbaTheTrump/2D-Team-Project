using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : NetworkBehaviour
{
    [SerializeField] MovementHandler _movementHandler;

    InventorySystem _inventory;

    //Input Actions
    [HideInInspector] PlayerInput _playerInput;
    [HideInInspector] public InputAction SprintAction;
    [HideInInspector] public InputAction WalkAction;

    private void Start()
    {
        _inventory = GetComponentInChildren<InventorySystem>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        enabled = IsOwner;
        SetActions();
    }

    void SetActions()
    {
        _playerInput = GetComponentInChildren<PlayerInput>();

        SprintAction = _playerInput.currentActionMap.FindAction("Sprint");

        _movementHandler.SetActions();
    }

    void OnMove(InputValue value)
    {
        _movementHandler.moveDir = value.Get<Vector2>();
    }

    void OnDrop()
    {
        DropItemServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    void DropItemServerRpc()
    {
        _inventory.TryRemoveItem(_inventory.SelectedSlot, true);
    }
}
