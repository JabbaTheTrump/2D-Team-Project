using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.Netcode;
using UnityEngine;

public class BiomassCollector : NetworkBehaviour, IInteractable
{
    [SerializeField] ItemData _sampleItemData;
    public NetworkVariable<bool> IsInteractable { get; set; } = new(true);

    public void Interact(NetworkObject interactor)
    {
        InventorySystem playerInv = interactor.GetComponentInChildren<InventorySystem>();

        if (playerInv == null)
        {
            Debug.LogWarning("A player with a null inventory attempted to interact with the collector: " + interactor.gameObject);
            return;
        }

        InventorySlot slot = playerInv.GetSlotByItemData(_sampleItemData);

        if (slot == null)
        {
            return;
        }

        Debug.Log("Inserting sample");

        playerInv.TryRemoveItem(slot.Index, false);
        GameStateController.Instance.AddSample();
    }
}
