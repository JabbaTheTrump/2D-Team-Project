using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class SceneManager : ServerSingleton<SceneManager>
{
    //[SerializeField] GameObject _playerPrefab; //TEMPORARY - SPAWNING PLAYER ON THIS SCRIPT FOR DEBUG PURPOSES

    void Start()
    {
        //LobbyStateManager.Instance.CurrentState.OnValueChanged += LobbyStateChanged;
        DontDestroyOnLoad(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkDespawn();
        NetworkManager.Singleton.SceneManager.OnLoadComplete += SceneFinishedLoading;
    }

    private void SceneFinishedLoading(ulong clientId, string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode)
    {
        Debug.Log("Load completed");
    //    if (sceneName == "Game") //TEMPORARY
    //    {
    //        Debug.Log("Spawning player");
    //        NetworkSpawnManager.Instance.SpawnPrefabOnNetwork(_playerPrefab, new(0,0));
    //    }
    //}

    void LoadScene(string sceneName) 
    {
        var status = NetworkManager.SceneManager.LoadScene(sceneName, loadSceneMode: UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    void LobbyStateChanged(string state)
    {
        if (state == "InGame")
        {
            LoadScene("Game");
        }
    }
}
