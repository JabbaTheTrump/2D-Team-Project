using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [SerializeField] AudioSource _FootstepsAudioSource;

    void Start()
    {
        LocalPlayerSpawnController.Instance.OnLocalPlayerSpawn += LocalPlayerSpawned;
    }

    void LocalPlayerSpawned(GameObject playerObject)
    {
        playerObject.GetComponent<PlayerMovementHandler>().onsp

    }

    void 

    // Update is called once per frame
    void Update()
    {
        
    }


}
