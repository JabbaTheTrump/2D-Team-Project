using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BiomassHarvester : NetworkBehaviour, IInteractable
{
    Biomass _biomassNode;

    HarvesterItemData _harvesterData;

    public NetworkVariable<bool> IsInteractable { get; set; } = new(false); //Set to Whether the harvester finished harvesting

    public void StartHarvester(Biomass mass, HarvesterItemData harvesterData)
    {
        _biomassNode = mass;
        _harvesterData = harvesterData;
        StartCoroutine(StartHarvester()); 
    }

    public void Interact(NetworkObject interactor)
    {
        if (!IsInteractable.Value) return;

        if (interactor.GetComponentInChildren<InventorySystem>().TryPickUpItem(_biomassNode.Sample))
        {
            IsInteractable.Value = false;
        }
    }

    IEnumerator StartHarvester()
    {
        Debug.Log("Starting Harvester");

        yield return new WaitForSeconds(_harvesterData.HarvestTime);

        Debug.Log("Harvester Finished");

        IsInteractable.Value = true;
    }
}
