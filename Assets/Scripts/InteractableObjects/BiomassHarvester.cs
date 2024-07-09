using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BiomassHarvester : NetworkBehaviour, IInteractable
{
    Biomass _biomassNode;

    HarvesterItemData _harvesterData;

    public NetworkVariable<bool> IsInteractable { get; set; } = new(false); //Set to Whether the harvester finished harvesting

    AudioSource _audioSource;

    public void StartHarvester(Biomass mass, HarvesterItemData harvesterData)
    {
        _biomassNode = mass;
        _harvesterData = harvesterData;
        _audioSource = GetComponentInChildren<AudioSource>();
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
        PlayDrillAudioClientRpc();

        AINoiseManager.Instance.CreateNoiseAtPoint(transform.position, _harvesterData.NoiseDistance, _harvesterData.HarvestTime);
        yield return new WaitForSeconds(_harvesterData.HarvestTime);

        PlayFinishAudioClientRpc();

        IsInteractable.Value = true;
    }

    [ClientRpc]
    void PlayDrillAudioClientRpc()
    {
        _audioSource.clip = _harvesterData._drillingAudioClip;
        _audioSource.Play();
    }

    [ClientRpc]
    void PlayFinishAudioClientRpc()
    {
        _audioSource.clip = _harvesterData._finishAudioClip;
        _audioSource.Play();
    }
}
