using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.Netcode;
using UnityEngine;

public class BiomassCollector : NetworkBehaviour, IInteractable
{
    [SerializeField] ItemData _sampleItemData;
    [SerializeField] AudioClip _successAudio;
    [SerializeField] AudioClip _failureAudio;
    AudioSource _audioSource;
    public NetworkVariable<bool> IsInteractable { get; set; } = new(true);

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

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
            PlayInteractionAudioClientRpc(false);
            return;
        }

        Debug.Log("Inserting sample");

        playerInv.TryRemoveItem(slot.Index, false);
        GameStateController.Instance.AddSample();
        PlayInteractionAudioClientRpc(true);
    }

    [ClientRpc] 
    public void PlayInteractionAudioClientRpc(bool success)
    {
        if (success) _audioSource.clip = _successAudio;
        else _audioSource.clip = _failureAudio;

        _audioSource.Play();
    }
}
