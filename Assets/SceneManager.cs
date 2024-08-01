using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class SceneManager : NetworkSingleton<SceneManager>
{
    public event Action<string> OnSceneFullyLoaded;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        NetworkManager.Singleton.SceneManager.OnLoadComplete += SceneFinishedLoading;
        LobbyStateManager.Instance.CurrentState.OnValueChanged += LobbyStateChanged;
        DontDestroyOnLoad(gameObject);
    }

    // IMPLEMENT THE PLAYER SPAWNING MECHANIC WITHIN THE SCENEFINISHEDLOADING METHOD!!!!!!
    private void SceneFinishedLoading(ulong clientId, string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode)
    {
        SceneFinishedLoadingClientRpc(clientId, sceneName);
    }

    void LoadScene(string sceneName) 
    {
        if (!IsServer)
        {
            Debug.LogWarning($"A non host player has attempted to load a scene using the scene manager! Player ID: {NetworkManager.LocalClientId}");
        }

        var status = NetworkManager.SceneManager.LoadScene(sceneName, loadSceneMode: UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    void LobbyStateChanged(string state)
    {
        if (state == "InGame")
        {
            LoadScene("Game");
        }
    }

    [ClientRpc]
    void SceneFinishedLoadingClientRpc(ulong clientId, string sceneName)
    {
        if (clientId == NetworkManager.LocalClientId)
        {
            Debug.Log("Load completed");
            OnSceneFullyLoaded?.Invoke(sceneName);
        }
    }
}
