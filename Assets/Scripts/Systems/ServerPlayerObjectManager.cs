using QFSW.QC;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ServerPlayerObjectManager : ServerSingleton<ServerPlayerObjectManager>
{
    public Dictionary<ulong, NetworkObject> PlayerObjects { get; private set; } = new();

    public bool TryAddPlayerObject(ulong playerId)
    {
        NetworkObject obj = NetworkManager.SpawnManager.GetPlayerNetworkObject(playerId);

        if (obj == null) return false;

        if (PlayerObjects.ContainsKey(playerId))
        {
            PlayerObjects[playerId] = obj;
        }
        else
        {
            PlayerObjects.Add(playerId, obj);
        }

        return true;
    }


    [Command]
    public void PrintPlayerObjectList() //DEBUG COMMAND
    {
        foreach (var pair in  PlayerObjects)
        {
            Debug.Log($"ID: {pair.Key}, OBJ: {pair.Value}");
        }
    }
}