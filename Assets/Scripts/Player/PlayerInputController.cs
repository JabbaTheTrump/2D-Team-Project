using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : NetworkBehaviour
{
    [SerializeField] MovementHandler movementHandler;

    InventorySystem _inventory;

    private void Start()
    {
        _inventory = GetComponentInChildren<InventorySystem>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        enabled = IsOwner;
    }

    void OnMove(InputValue value)
    {
        Debug.Log("move");
        movementHandler.moveDir = value.Get<Vector2>();
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
