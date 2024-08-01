using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Netcode;
using UnityEngine;

public class NetworkSpawnManager : ServerSingleton<NetworkSpawnManager>
{
    public NetworkObject SpawnPrefabOnNetwork(GameObject prefab, Vector2 position)
    {
        if (prefab == null)
        {
            Debug.LogWarning("Attempted to spawn a null prefab");
            return null;
        }

        NetworkObject netObject = Instantiate(prefab, position, Quaternion.identity).GetComponent<NetworkObject>();

        if (netObject == null)
        {
            Debug.Log($"Attempted to spawn an object that lacks a networkobject component on the server! {prefab}");
            return null;
        }

        netObject.Spawn();
        return netObject;
    }

    public void SpawnPlayerObjectOnNetwork(GameObject playerPrefab, ulong clientId, Vector2 position)
    {
        GameObject playerObj = Instantiate(playerPrefab, position, Quaternion.identity);
        playerObj.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
    }

    public void DespawnObjectOnNetwork(NetworkObject netObject)
    {
        if (netObject == null)
        {
            Debug.Log($"Attempted to despawn an object that doesn't exist on the server!");
            return;
        }

        netObject.Despawn();
    }
}
