using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class UI_LobbyListManager : MonoBehaviour
{
    [SerializeField] GameObject _lobbyInstanceUIElementPrefab;
    [SerializeField] Transform _lobbyListContainer;
    [SerializeField] ToggleGroup _listToggleGroup;
    [SerializeField] GameObject _lobbyListMenuContainer;
    public List<GameObject> _lobbyUIInstances;
    

    void Start()
    {
        LobbyManager.Instance.CurrentLobby.OnValueChanged += OnLobbyChanged;
        StartCoroutine(RefreshListOnStart());
    }

    void OnLobbyChanged(Lobby lobby)
    {
        if (lobby == null)
        {
            _lobbyListMenuContainer.SetActive(true);
            RefreshList();
        }

        else _lobbyListMenuContainer.SetActive(false);
    }

    public async void RefreshList()
    {
        RemoveAllServerUIElements();

        try
        {
            List<Lobby> querriedLobbies = await LobbyManager.Instance.GetLobbyList(); //Gets a list of all the lobbies

            foreach (var lobby in querriedLobbies)
            {
                if (lobby == null) continue;

                GameObject lobbyUIElemenet = Instantiate(_lobbyInstanceUIElementPrefab, _lobbyListContainer); //Creates a UI element for the lobby
                lobbyUIElemenet.GetComponent<UI_LobbyInstance>().SetLobby(lobby); //sets the lobby on the UI (in order to update the UI)

                Toggle lobbyToggle = lobbyUIElemenet.GetComponent<Toggle>();
                lobbyToggle.group = _listToggleGroup; //Adds the toggle elemeny to the toggle group

                _lobbyUIInstances.Add(lobbyUIElemenet); //Adds it to the list of UI elements
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public Lobby GetSelectedLobby() //Returns the lobby of the currently selected UI element
    {
        Toggle selectedToggle = _listToggleGroup. //Gets the activated toggle from the toggle group
            ActiveToggles().
            FirstOrDefault();

        if (selectedToggle == null) return null; //Returns null if no toggle is enabled

        Lobby lobby = selectedToggle.GetComponentInChildren<UI_LobbyInstance>().Lobby; 
        if (lobby == null) Debug.LogWarning("A UI element has a null lobby value!");

        return lobby;
    }

    void RemoveAllServerUIElements()
    {
        List<GameObject> temp = new(_lobbyUIInstances);

        foreach (var element in temp)
        {
            _lobbyUIInstances.Remove(element);
            Destroy(element);
        }
    }

    IEnumerator RefreshListOnStart()
    {
        yield return null;
        RefreshList();
    }
}
