using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawnController : NetworkBehaviour
{
    public static event Action<GameObject> OnLocalPlayerSpawn;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsLocalPlayer)
        {
            StartCoroutine(EventFireTest());
        }
    }

    IEnumerator EventFireTest()
    {
        yield return null;
        OnLocalPlayerSpawn?.Invoke(gameObject);
    }
}
