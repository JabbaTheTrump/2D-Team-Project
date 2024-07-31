using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class UI_LobbyMenuButtons : MonoBehaviour
{
    [SerializeField] Button _createBtn;
    [SerializeField] Button _joinBtn;
    [SerializeField] Button _refreshBtn;
    [SerializeField] Button _startBtn;
    [SerializeField] Button _leaveBtn;


    [SerializeField] UI_LobbyListManager _lobbyListManager;

    private void Start()
    {
        _createBtn.onClick.AddListener(() => LobbyManager.Instance.CreateLobby("hi"));
        _refreshBtn.onClick.AddListener(_lobbyListManager.RefreshList);
        _joinBtn.onClick.AddListener(JoinButtonClicked);
        _leaveBtn.onClick.AddListener(LobbyManager.Instance.LeaveLobby);
        _startBtn.onClick.AddListener(StartButtonClicked);

        LobbyManager.Instance.CurrentLobby.OnValueChanged += (_) => ChangeActiveButtons();

        ChangeActiveButtons();
    }

    void JoinButtonClicked() //Attempts to join the selected lobby
    {
        Lobby lobby = _lobbyListManager.GetSelectedLobby();

        if (lobby == null) return;

        LobbyManager.Instance.JoinLobby(lobby.Id);
    }

    void StartButtonClicked()
    {
        LobbyStateManager.Instance.ChangeLobbyState("InGame");
    }

    void ChangeActiveButtons() //Enables / disables relevant buttons in the menu according to the current UI state (i.e in lobby, in lobby list)
    {
        if (LobbyManager.Instance.CurrentLobby.Value == null) //If the player isn't in a lobby
        {
            _createBtn.gameObject.SetActive(true);
            _joinBtn.gameObject.SetActive(true);
            _refreshBtn.gameObject.SetActive(true);

            _startBtn.gameObject.SetActive(false);
            _leaveBtn.gameObject.SetActive(false);
            return;
        }
        //If the player IS in a lobby

        _leaveBtn.gameObject.SetActive(true);

        _createBtn.gameObject.SetActive(false);
        _joinBtn.gameObject.SetActive(false);
        _refreshBtn.gameObject.SetActive(false);

        if (LobbyManager.Instance.IsHost)
        {
            _startBtn.gameObject.SetActive(true);
        }
    }
}
