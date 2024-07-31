using UnityEngine;
using Unity.Netcode;
using System;

public class PlayerObjectManager : NetworkSingleton<PlayerObjectManager>
{
    [SerializeField] GameObject _playerPrefab;
    public event Action<GameObject> OnLocalPlayerSpawned;

    public void SpawnPlayerInPosition(Vector2 position)
    {
        NetworkObject playerNetworkObject = NetworkSpawnManager.Instance.SpawnPrefabOnNetwork(_playerPrefab, position);
        GameObject playerObject = playerNetworkObject.gameObject;

        OnLocalPlayerSpawned?.Invoke(playerObject);
    }
}