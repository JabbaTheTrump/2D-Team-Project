using UnityEngine;
using UnityEngine.Rendering.UI;

public class MovementAudioHandler : MonoBehaviour
{
    [SerializeField] MovementHandler _movementHandler;
    [SerializeField] AudioSource _movementAudioSource;

    private void Start()
    {
        if (_movementHandler == null || _movementAudioSource == null)
        {
            Debug.LogWarning($"{gameObject}'s MovementAudioHandler's parameters aren't assigned!");
            return;
        }

        _movementHandler.CurrentMovementState.OnValueChanged += ChangeAudioClip;
        ChangeAudioClip(_movementHandler.CurrentMovementState.Value);
    }

    void ChangeAudioClip(MovementState state)
    {
        AudioClip audioClip = _movementHandler.GetMovementTypeByState(state)?.MovementStateAudio;

        if (audioClip == null)
        {
            _movementAudioSource.Stop();
            return;
        }

        _movementAudioSource.clip = audioClip;
        _movementAudioSource.Play();

        _movementAudioSource.time = Random.Range(0, audioClip.length);
    }
}