using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    //                        Sprinting                      //
    [Header("Footsteps Audio")]
    [SerializeField] AudioSource _footstepsAudioSource;
    [SerializeField] AudioClip _walkAudioClip;
    [SerializeField] AudioClip _sprintAudioClip;

    //Private fields
    float _walkAudioProgress = 0;
    float _sprintAudioProgress = 0;

    //                                                       //

    void Start()
    {
        LocalPlayerSpawnController.Instance.OnLocalPlayerSpawn += LocalPlayerSpawned;
    }

    void LocalPlayerSpawned(GameObject playerObject)
    {
        playerObject.GetComponent<PlayerMovementHandler>().CurrentMovementState.OnValueChanged += SetPlayerMovementAudio;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetPlayerMovementAudio(MovementState state)
    {
        switch (state)
        {
            case MovementState.Idle:
                if (_footstepsAudioSource.clip == _walkAudioClip) _walkAudioProgress = _footstepsAudioSource.time;
                else if (_footstepsAudioSource.clip == _sprintAudioClip) _sprintAudioProgress = _footstepsAudioSource.time;
                _footstepsAudioSource.Stop();
                break;

            case MovementState.Walking:
                _footstepsAudioSource.clip = _walkAudioClip;
                _footstepsAudioSource.time = _walkAudioProgress;
                _footstepsAudioSource.Play();
                break;

            case MovementState.Sprinting:
                _footstepsAudioSource.clip = _sprintAudioClip;
                _footstepsAudioSource.time = _sprintAudioProgress;
                _footstepsAudioSource.Play();
                break;
        }
    }
}
