using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class UI_PlayerList : MonoBehaviour
{
    Lobby _lobby;

    [SerializeField] GameObject _playerInstanceUIElementPrefab;
    Dictionary<string, GameObject> _playerUIInstances = new();

    [SerializeField] Transform _playerListContainer;

    private void Start()
    {
        LobbyManager.Instance.EventCallbacks.PlayerJoined += OnPlayerJoined;
        LobbyManager.Instance.EventCallbacks.PlayerLeft += OnPlayerLeft;
    }

    private void OnEnable()
    {
        _lobby = LobbyManager.Instance.CurrentLobby.Value;
        DrawPlayerList();
    }

    void OnPlayerJoined(List <LobbyPlayerJoined> players)
    {
        foreach (var player in players)
        {
            AddPlayerElement(player.Player);
        }
    }

    void OnPlayerLeft(List<int> players)
    {
        foreach (var index in players)
        {
            RemovePlayerElement(_lobby.Players[index].Id);
        }
    }

    void AddPlayerElement(Player player)
    {
        _playerUIInstances.Add(player.Id, Instantiate(_playerInstanceUIElementPrefab, _playerListContainer));
        _playerUIInstances[player.Id].GetComponentInChildren<TextMeshProUGUI>().text = player.Id;
    }

    void RemovePlayerElement(string id)
    {
        Destroy(_playerUIInstances[id]);
        _playerUIInstances.Remove(id);
    }

    void DrawPlayerList()
    {
        Dictionary<string, GameObject> temp = new(_playerUIInstances);

        foreach (var entry in temp)
        {
            RemovePlayerElement(entry.Key);
        }

        foreach (var player in _lobby.Players)
        {
            AddPlayerElement(player);
        }
    }
}
