using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class UI_LobbyInstance : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _lobbyName;
    [SerializeField] TextMeshProUGUI _playerCount;

    public Lobby Lobby;

    public void SetLobby(Lobby lobby)
    {
        Lobby = lobby;

        if (lobby == null)
        {
            Debug.LogWarning("A lobby UI instance has a recieved a null lobby reference.");
            return;
        }

        _lobbyName.text = lobby.Name;
        _playerCount.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
    }
}
