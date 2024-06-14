using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AIAudioManager : MonoBehaviour
{
    public AIMovement AIMovementManager;

    [SerializeField] AudioSource _movementAudioSource;
    [SerializeField] AudioClip _walkAudioClip;
    [SerializeField] AudioClip _runAudioClip;

    [SerializeField] AudioClip _attackAudioClip;

    private void Start()
    {
        AIMovementManager.IsWalking.OnValueChanged += UpdateMovementAudio;
        AIMovementManager.IsChasing.OnValueChanged += UpdateMovementAudio;
        UpdateMovementAudio(true, true);
    }

    void UpdateMovementAudio(bool _, bool __)
    {
        if (AIMovementManager.IsWalking.Value)
        {
            _movementAudioSource.clip = _walkAudioClip;
            _movementAudioSource.Play();
        }

        else if (AIMovementManager.IsChasing.Value)
        {
            _movementAudioSource.clip = _runAudioClip;
            _movementAudioSource.Play();
        }
        else
        {
            _movementAudioSource.Pause();
        }
    }
}
