using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Biomass : NetworkBehaviour, IInteractable
{
    [field: SerializeField] public Item Sample { get; set; }

    NetworkVariable<bool> _harvestable = new(true);

    public void Interact(NetworkObject interactor)
    {
        InventorySystem inventory = interactor.GetComponentInChildren<InventorySystem>();

        InventorySlot harvesterSlot = inventory.GetSlotByItemType<HarvesterItemData>();

        if (harvesterSlot == null) return; //Returns if the player doesn't have a harvester

        if (inventory.TryRemoveItem(harvesterSlot.Index, false)) 
        {
            //SetUpHarvester();
        }
    }

    void SetUpHarvester(HarvesterItemData harvester) //Spawns the harvester
    {
        if (!_harvestable.Value)
        {
            Debug.LogWarning($"Attempted to place a harvester on a node marked as unharvestable {gameObject}");
            return;
        }

        _harvestable.Value = false;

        
    }
}
