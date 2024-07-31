using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class UI_LobbyScreenManager : MonoBehaviour
{
    [SerializeField] GameObject _lobbyScreen;
    [SerializeField] TextMeshProUGUI _lobbyNameTextBox;
    [SerializeField] UI_PlayerList _playerListManager;

    Lobby _lobby;

    // Start is called before the first frame update
    void Start()
    {
        _lobbyScreen.SetActive(false);
        LobbyManager.Instance.CurrentLobby.OnValueChanged += LobbyStatusChanged;
    }

    void LobbyStatusChanged(Lobby newLobby)
    {
        _lobby = newLobby;

        if (_lobby == null)
        {
            _lobbyScreen.SetActive(false);
        }
        else
        {
            _lobbyScreen.SetActive(true);
            _lobbyNameTextBox.text = newLobby.Name;
        }
    }
}
