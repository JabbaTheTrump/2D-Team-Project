using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyStateManager : Singleton<LobbyStateManager>
{
    Lobby CurrentLobby
    {
        set
        {
            LobbyManager.Instance.CurrentLobby.Value = value;
        }
        get
        {
            return LobbyManager.Instance.CurrentLobby.Value;
        }
    }

    public ObservableVariable<string> CurrentState = new("");

    // Start is called before the first frame update
    void Start()
    {
        LobbyManager.Instance.EventCallbacks.DataChanged += OnLobbyDataChanged;
        LobbyManager.Instance.CurrentLobby.OnValueChanged += LobbyChanged;
    }

    private void LobbyChanged(Lobby newLobby)
    {
        if (newLobby == null) CurrentState.Value = "";
        else if (newLobby.Data.ContainsKey("GameState"))
        {
            CurrentState.Value = newLobby.Data["GameState"].Value;
        }
    }

    private void OnLobbyDataChanged(Dictionary<string, ChangedOrRemovedLobbyValue<DataObject>> dictionary)
    {
        if (dictionary.ContainsKey("GameState"))
        {
            CurrentState.Value = dictionary["GameState"].Value.Value;
        }
    }

    public async void ChangeLobbyState(string newState)
    {
        if (CurrentState.Value != newState)
        {
            try
            {
                UpdateLobbyOptions options = new()
                {
                    Data = new Dictionary<string, DataObject>()
                    {
                        {
                            "GameState", new DataObject(
                                visibility: DataObject.VisibilityOptions.Public,
                                value: newState)
                        }
                    }
                };

                CurrentLobby = await LobbyService.Instance.UpdateLobbyAsync(CurrentLobby.Id, options);
            }

            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }
}
