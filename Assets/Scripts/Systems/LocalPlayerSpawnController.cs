using Newtonsoft.Json;
using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class LocalPlayerSpawnController : Singleton<LocalPlayerSpawnController>
{
    public event Action<GameObject> OnLocalPlayerSpawn;
    public NetworkObject PlayerObject;

 
    public IEnumerator InvokeLocalPlayerEventNextFrame(GameObject playerObject)
    {
        yield return null;
        OnLocalPlayerSpawn?.Invoke(playerObject);
        Debug.Log("<color=green>Invoking local player spawn event!</color>");
        PlayerObjectSpawnedServerRpc(playerObject.GetComponent<NetworkObject>().OwnerClientId);
    }

    [ServerRpc (RequireOwnership = false)]
    public void PlayerObjectSpawnedServerRpc(ulong playerId)
    {
        if (!ServerPlayerObjectManager.Instance.TryAddPlayerObject(playerId))
            Debug.LogWarning($"Failed to add PlayerObject to server! Player id: {playerId}");
    }
}
