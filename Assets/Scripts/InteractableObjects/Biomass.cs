using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Biomass : NetworkBehaviour, IInteractable
{
    [field: SerializeField] public Item Sample { get; set; }
    public NetworkVariable<bool> IsInteractable { get; set; } = new(true);
        public NetworkVariable<bool> BeingInteractedWith { get; set; } = new(false);
    [field: SerializeField] public float InteractionTime { get; set; }

    [SerializeField] GameObject _harvesterPrefab;

    public void Interact(NetworkObject interactor)
    {
        InventorySystem inventory = interactor.GetComponentInChildren<InventorySystem>();

        InventorySlot harvesterSlot = inventory.GetSlotByItemType<HarvesterItemData>();

        if (harvesterSlot == null) return; //Returns if the player doesn't have a harvester
        //Debug.Log("Harvester detected in inventory");

        SetUpHarvester(harvesterSlot.Item.ItemData as HarvesterItemData);

        inventory.TryRemoveItem(harvesterSlot.Index, false);
    }

    void SetUpHarvester(HarvesterItemData harvesterData) //Spawns the harvester and initializes it
    {
        if (harvesterData == null)
        {
            Debug.LogWarning("Attempted to plaec a harvester without harvester data");
            return;
        }
        //Debug.Log("Settings up harvester");

        NetworkObject placedHarvester = NetworkSpawnManager.Instance.SpawnPrefabOnNetwork(_harvesterPrefab, transform.position);
        BiomassHarvester harvesterController = placedHarvester.GetComponentInChildren<BiomassHarvester>();

        if (harvesterController == null)
        {
            Debug.LogWarning("A placed harvester does not contain a controller");
            placedHarvester.Despawn();
            return;
        }

        IsInteractable.Value = false;

        harvesterController.StartHarvester(this, harvesterData);
    }
}
