using QFSW.QC.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;
using System;

public class PlayerStateManager : ServerSingleton<PlayerStateManager>
{
    [SerializeField] List<PlayerStateEntry> PlayerStateList;


    [System.Serializable]
    public struct PlayerStateEntry
    {
        public ulong PlayerId;
        public PlayerState State;
    }

    public enum PlayerState
    {
        NotSpawned,
        Spawned_Alive,
        Spawned_Dead
    }

    public event Action<List<PlayerStateEntry>> OnPlayerStateListChange;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        NetworkManager.OnClientConnectedCallback += AddPlayerEntry;
        NetworkManager.OnClientDisconnectCallback += RemovePlayerEntry;
    }

    void AddPlayerEntry(ulong id)
    {
        PlayerStateList.Add(
            new PlayerStateEntry
            {
                PlayerId = id,
                State = PlayerState.Spawned_Alive
            });

        NetworkObject playerObject = NetworkManager.SpawnManager.GetPlayerNetworkObject(id);
        playerObject.GetComponent<HealthSystem>().OnDeath += () => ChangePlayerState(id, PlayerState.Spawned_Dead);
    }

    void RemovePlayerEntry(ulong id)
    {
        PlayerStateEntry entry = PlayerStateList.Where(entry => entry.PlayerId == id).FirstOrDefault();

        if (entry.Equals(default(PlayerStateEntry)))
        {
            Debug.LogWarning($"Attempted to remove a player entry for an ID which isn't registered. ID: {id}");
            return;
        }

        PlayerStateList.Remove(entry);
    }

    void ChangePlayerState(ulong id, PlayerState state)
    {
        PlayerStateEntry entry = PlayerStateList.Where(entry => entry.PlayerId == id).FirstOrDefault();

        if (entry.Equals(default(PlayerStateEntry)))
        {
            Debug.LogWarning($"Attempted to modify a player's entry for an ID which isn't registered. ID: {id}");
            return;
        }

        entry.State = state;
    }
}
