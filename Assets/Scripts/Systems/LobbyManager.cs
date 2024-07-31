using QFSW.QC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Rendering;

public class LobbyManager : Singleton<LobbyManager>
{
    public ObservableVariable<Lobby> CurrentLobby { get; private set; } = new(null);
    private string _localPlayerId; //Caches the local player ID, since the anonymous sign-in overwrites it.

    public bool IsHost
    {
        get
        {
            return CurrentLobby.Value != null //Returns false if the player isn't in a lobby
                && AuthenticationService.Instance != null //Returns false if the authentication service has shut down
                && CurrentLobby.Value.HostId == _localPlayerId; //Returns whether the player's ID equals to that of the lobby's host
        }
    }

    [HideInInspector] public LobbyEventCallbacks EventCallbacks = new();

    private Coroutine _heartbeatCoroutine;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();

        try
        {
            SignIn();
        }
        catch (AuthenticationException e)
        {
            Debug.Log(e);
        }

        EventCallbacks.LobbyDeleted += () => {CurrentLobby.Value = null; };
    }

    async void SignIn()
    {
        await UnityServices.InitializeAsync(); //Initializes unity services

        AuthenticationService.Instance.ClearSessionToken(); //Ensures that the cache is clear from previous anonymous sign ins

        await AuthenticationService.Instance.SignInAnonymouslyAsync(); //Signs in

        Debug.Log($"Signed in as {AuthenticationService.Instance.PlayerId}!");
        _localPlayerId = AuthenticationService.Instance.PlayerId;
    }

    public async void CreateLobby(string name)
    {
        if (CurrentLobby.Value != null) return;

        try
        {
            string relayJoinCode = await RelayManager.Instance.StartHostWithRelay();

            CreateLobbyOptions options = new()
            {
                Data = new Dictionary<string, DataObject>()
                {
                    {
                        "GameState", new DataObject(
                            visibility: DataObject.VisibilityOptions.Public,
                            value:"InLobby")
                    },
                    {
                        "RelayJoinCode", new DataObject(
                            visibility: DataObject.VisibilityOptions.Public,
                            value: relayJoinCode)
                    }
                }
            };

            CurrentLobby.Value = await LobbyService.Instance.CreateLobbyAsync(name, 4, options);
            _heartbeatCoroutine = StartCoroutine(HandleLobbyHeartbeat(15)); //Starts the heartbeat system in order to keep the lobby running
            await Lobbies.Instance.SubscribeToLobbyEventsAsync(CurrentLobby.Value.Id, EventCallbacks); //Subscribes to the lobby event

            Debug.Log($"Created new lobby with the name {name} and ID: {CurrentLobby.Value.Id}");
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void JoinLobby(string id)
    {
        try
        {
            CurrentLobby.Value = await LobbyService.Instance.JoinLobbyByIdAsync(id); //Joins the lobby
            await RelayManager.Instance.StartClientWithRelay(CurrentLobby.Value.Data["RelayJoinCode"].Value);
            await Lobbies.Instance.SubscribeToLobbyEventsAsync(id, EventCallbacks); //Subscribes to the lobby events
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    public async void LeaveLobby()
    {
        //Debug.Log($"Leaving lobby. IsHost: {IsHost}, hostId: {CurrentLobby.Value.HostId}, playerId: {_localPlayerId}");

        if (CurrentLobby.Value == null) return;

        if (IsHost) //Deletes the lobby instead of leaving it if the player is the host
        {
            DeleteLobby();
            return;
        }
        
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(CurrentLobby.Value.Id, _localPlayerId); //Remove him from the lobby
            CurrentLobby.Value = null;
            Debug.Log("Left lobby!");
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    public async void DeleteLobby()
    {
        if (CurrentLobby.Value == null) return;

        try
        {
            StopLobbyHeartbeat();   
            await LobbyService.Instance.DeleteLobbyAsync(CurrentLobby.Value.Id);
            CurrentLobby.Value = null;

            Debug.Log("Lobby deleted!");
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    
    void StopLobbyHeartbeat()
    {
        if (_heartbeatCoroutine != null) StopCoroutine(_heartbeatCoroutine);
    }

    IEnumerator HandleLobbyHeartbeat(float secondsBetweenHeartbeat)
    {
        while (true)
        {
            yield return new WaitForSeconds(secondsBetweenHeartbeat);
            Debug.Log("Sending heartbeat...");
            LobbyService.Instance.SendHeartbeatPingAsync(CurrentLobby.Value.Id);
        }
    }

    [Command]
    public async Task<List<Lobby>> GetLobbyList()
    {
        try
        {
            QueryResponse response = await Lobbies.Instance.QueryLobbiesAsync();
            List<Lobby> lobbies = response.Results;

            return lobbies;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
        return null;
    }

    async void OnApplicationQuit()
    {
        try
        {
            if (IsHost)
            {
                await LobbyService.Instance.DeleteLobbyAsync(CurrentLobby.Value.Id);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
}
