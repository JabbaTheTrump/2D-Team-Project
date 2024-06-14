using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BiomassHarvester : NetworkBehaviour, IInteractable
{
    Biomass _biomassNode;

    HarvesterItemData _harvesterData;

    //AudioClip _drillingAudioClip;
    //AudioClip _finishAudioClip;

    public NetworkVariable<bool> IsInteractable { get; set; } = new(false); //Set to Whether the harvester finished harvesting

    public void StartHarvester(Biomass mass, HarvesterItemData harvesterData)
    {
        _biomassNode = mass;
        _harvesterData = harvesterData;
        StartCoroutine(StartHarvester()); 
    }

    public void Interact(NetworkObject interactor)
    {
        if (interactor.GetComponentInChildren<InventorySystem>().TryPickUpItem(_biomassNode.Sample))
        {
            IsInteractable.Value = false;
        }
    }

    IEnumerator StartHarvester()
    {
        yield return new WaitForSeconds(_harvesterData.HarvestTime);

        IsInteractable.Value = true;
    }
}
