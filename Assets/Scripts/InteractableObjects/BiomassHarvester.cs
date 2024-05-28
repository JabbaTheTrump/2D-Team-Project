using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BiomassHarvester : NetworkBehaviour, IInteractable
{
    NetworkVariable<bool> _finished = new(false); //Whether the harvester finished harvesting
    NetworkVariable<bool> _containsResource = new(true); //Whether the item were picked up from the harvester
    Biomass _biomassNode;

    HarvesterItemData _harvesterData;

    public void StartHarvester(Biomass mass, HarvesterItemData harvesterData)
    {
        _biomassNode = mass;
        _harvesterData = harvesterData;
        StartCoroutine(StartHarvester()); 
    }

    public void Interact(NetworkObject interactor)
    {
        if (!_finished.Value || !_containsResource.Value) return;

        if (interactor.GetComponentInChildren<InventorySystem>().TryPickUpItem(_biomassNode.Sample.ItemData))
        {
            _containsResource.Value = false;
        }
    }

    IEnumerator StartHarvester()
    {
        yield return new WaitForSeconds(_harvesterData.HarvestTime);
        _finished.Value = true;
    }
}
