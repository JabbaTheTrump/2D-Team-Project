using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using Unity.Netcode;
using UnityEngine;

public class LocalPlayerObjectManager : NetworkSingleton<LocalPlayerObjectManager>
{
    [SerializeField] GameObject _playerPrefab;

    public event Action<GameObject> OnLocalPlayerSpawned;
    public GameObject PlayerObject;

    override protected void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        SceneManager.Instance.OnSceneFullyLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(string sceneName)
    {
        if (sceneName == "Game")
        {
            //PlayerObject = Instantiate(_playerPrefab);
            //StartCoroutine(InvokePlayerSpawnEvent());

            //SpawnPlayerInPosition(new(0, 0));
        }
    }

    public void SpawnPlayerInPosition(Vector2 position)
    {
        SpawnPlayerServerRpc(NetworkManager.LocalClientId, position);
        PlayerObject = NetworkManager.LocalClient.PlayerObject.gameObject;
        StartCoroutine(InvokePlayerSpawnEvent()); //Invokes the spawn event with a 1 frame delay to allow the player scripts to initialize
    }

    public IEnumerator InvokePlayerSpawnEvent() 
    {
        yield return null;
        Debug.Log($"Invoked player spawn event on client");
        OnLocalPlayerSpawned?.Invoke(PlayerObject);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnPlayerServerRpc(ulong clientId, Vector2 position)
    {
        Debug.Log($"Player with ID {clientId} has spawned a player object");
        NetworkSpawnManager.Instance.SpawnPlayerObjectOnNetwork(_playerPrefab, clientId, position);
        PlayerStateManager.Instance.PlayerObjects.Add(NetworkManager.ConnectedClients[clientId].PlayerObject); //TEMPORARY - CACHES THE PLAYER OBJECT FOR AI PURPOSES
    }
}