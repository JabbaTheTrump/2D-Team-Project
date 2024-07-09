using Unity.Netcode;
using UnityEngine;

public class SoundFXManager : Singleton<SoundFXManager>
{

    public void PlaySoundAtPoint(AudioClip audioClip, Vector2 position, float volume) //Plays a random sound at a point
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volume);
    }

    public void PlaySoundAtPoint(AudioClip[] audioClips, Vector2 position, float volume) //Plays a random sound out of an array at a point
    {
        AudioClip randomClip = audioClips[Random.Range(0, audioClips.Length)];
        PlaySoundAtPoint(randomClip, position, volume);
    }
}