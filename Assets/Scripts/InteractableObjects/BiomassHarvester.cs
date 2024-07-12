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

    public bool _resourcePickedUp = false;

    public void StartHarvester(Biomass mass, HarvesterItemData harvesterData)
    {
        _audioSource = GetComponentInChildren<AudioSource>();
        _biomassNode = mass;
        _harvesterData = harvesterData;
        StartCoroutine(StartHarvester()); 
    }

    public void Interact(NetworkObject interactor)
    {
        if (interactor.GetComponentInChildren<InventorySystem>().TryPickUpItem(_biomassNode.Sample) && !_resourcePickedUp)
        {
            _resourcePickedUp = true;
            IsInteractable.Value = false;
        }
    }

    IEnumerator StartHarvester()
    {
        SetUpHarvesterClientRpc(_harvesterData.Id);
        PlayDrillAudioClientRpc();

        AINoiseManager.Instance.CreateNoiseAtPoint(transform.position, _harvesterData.NoiseDistance, _harvesterData.HarvestTime);
        yield return new WaitForSeconds(_harvesterData.HarvestTime);

        PlayFinishAudioClientRpc();

        IsInteractable.Value = true;
    }

    [ClientRpc]
    void SetUpHarvesterClientRpc(int harvesterDataId)
    {
        _audioSource = GetComponentInChildren<AudioSource>();
        _harvesterData = ItemDictionary.Instance.GetItemDataById(harvesterDataId) as HarvesterItemData;
        

        if (_harvesterData == null)
        {
            Debug.LogWarning("Client does not have a valid harvesterdata!");
            return;
        }
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
