using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.Android;

public class BaseAIAudioManager : ServerSideNetworkedBehaviour
{
    // Private fields
    [SerializeField] WalkerAI _aiHandler;
    bool _canMakeAggroSound = true;
    //

    [Header("Idle Audio")]
    [SerializeField] AudioClip[] _idleAudioClips;
    [SerializeField] float _minimumDelay;
    [SerializeField] float _maximumDelay;

    [Space]

    [Header("Attack Audio")]
    [SerializeField] float _attackAudioDelay = 0.5f;
    [SerializeField] AudioClip[] _attackAudioClips;


    [Space]

    [Header("Aggro Audio")]
    [SerializeField] float _aggroSoundCooldown;
    [SerializeField] AudioClip[] _aggroAudioClips;

    [Space]

    [SerializeField] AudioSource _vocalAudioSource; //The audio source that plays entity noises like growls, moaning and breathing

    private void Start()
    {
        _aiHandler = GetComponentInParent<WalkerAI>();
        _aiHandler.CurrentState.OnValueChanged += HandleAudioOnAIStateChanged;
        _aiHandler.OnAttack += () => StartCoroutine(PlayAttackAudioAfterPeriod());
        StartCoroutine(PlayIdleAudioAfterPeriod());
    }
    
    private void HandleAudioOnAIStateChanged(WalkerAI.State state)
    {
        if (state == WalkerAI.State.Roam)
        {
            _vocalAudioSource.volume = 0.6f;
            StartCoroutine(PlayIdleAudioAfterPeriod());
            return;
        }

        else if (state == WalkerAI.State.Chase)
        {
            if (_canMakeAggroSound)
            {
                _vocalAudioSource.volume = 1f;
                PlayAggroAudioClientRpc(Random.Range(0, _aggroAudioClips.Length));
                StartCoroutine(StartAggroSoundCooldown());
            }
        }
    }

    IEnumerator StartAggroSoundCooldown()
    {
        _canMakeAggroSound = false;
        yield return new WaitForSeconds(_aggroSoundCooldown);
        _canMakeAggroSound = true;
    }

    IEnumerator PlayIdleAudioAfterPeriod()
    {
        yield return new WaitForSeconds(Random.Range(_minimumDelay, _maximumDelay));

        if (_aiHandler.CurrentState.Value == WalkerAI.State.Roam) //Only plays the idle sound if the enemy is actually idle
        {
            PlayIdleAudioClientRpc(Random.Range(0, _idleAudioClips.Length));
            StartCoroutine(PlayIdleAudioAfterPeriod());
        }
    }

    IEnumerator PlayAttackAudioAfterPeriod()
    {
        yield return new WaitForSeconds(_attackAudioDelay);
        PlayAttackAudioClientRpc(Random.Range(0, _attackAudioClips.Length));
    }


    [ClientRpc]
    void PlayIdleAudioClientRpc(int index)
    {
        if (0 > index || index >= _idleAudioClips.Length)
        {
            Debug.LogWarning($"Incorrect index detected in AI Audio Manager belonging to {transform.root.parent}");
            return;
        }

        _vocalAudioSource.clip = _idleAudioClips[index];
        _vocalAudioSource.Play();
        Debug.Log("Playing idle sound");
    }

    [ClientRpc]
    void PlayAggroAudioClientRpc(int index)
    {
        if (0 > index || index >= _aggroAudioClips.Length)
        { 
            Debug.LogWarning($"Incorrect index detected in AI Audio Manager belonging to {transform.root.parent}");
            return;
        }

        _vocalAudioSource.clip = _aggroAudioClips[index];
        _vocalAudioSource.Play();
        Debug.Log("Playing aggro sound");
    }

    [ClientRpc]
    void PlayAttackAudioClientRpc(int index)
    {
        if (0 > index || index >= _attackAudioClips.Length)
        {
            Debug.LogWarning($"Incorrect index detected in AI Audio Manager belonging to {transform.root.parent}");
            return;
        }

        _vocalAudioSource.clip = _attackAudioClips[index];
        _vocalAudioSource.Play();
        Debug.Log("Playing attack sound");
    }
}

//Try to overhaul by creating a single "audiable" class that dynamically instantiates audio sources, and activate it using unity events

//public class Audiable : ServerSideNetworkedBehaviour
//{
//    [SerializeField] AudioSource _dynamicAudioSource;

//    [SerializeField] AudioClip[] _audioClips;
    

//    public void PlayAudio()
//    {
//        PlayAudioOnClient(Random.Range(0, _audioClips.Length));
//    }


//    [ClientRpc] 
//    void PlayAudioOnClient(int index)
//    {
//        if (0 > index || index >= _audioClips.Length)
//        {
//            Debug.LogWarning($"Incorrect index detected in AI Audio Manager belonging to {transform.root.parent}");
//            return;
//        }
//    }
//}