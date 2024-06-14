using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TestingNetworkStarter : ServerSingleton<TestingNetworkStarter>
{
    [SerializeField] Button _hostBtn;
    [SerializeField] Button _joinBtn;
    [SerializeField] TextMeshProUGUI _relayIdField;

    private void Start()
    {
        _hostBtn.onClick.AddListener(OnHostLobbyBtnPressed);
        _joinBtn.onClick.AddListener(OnJoinLobbyBtnPressed);
    }

    public async void OnHostLobbyBtnPressed()
    {
        Debug.Log(await StartHostWithRelay());
        Destroy(_hostBtn.transform.parent.gameObject);
    }

    public async void OnJoinLobbyBtnPressed()
    {
        if (_relayIdField.text != "")
        {
           if (await StartClientWithRelay(_relayIdField.text.Substring(0, 6)))
           {
                Destroy(_hostBtn.transform.parent.gameObject);
            }
        }
    }

    public async Task<string> StartHostWithRelay(int maxConnections = 5)
    {
        try
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return NetworkManager.Singleton.StartHost() ? joinCode : null;
        } catch (RelayServiceException ex)
        {
            Debug.Log(ex.Message);
            return "";
        }
    }

    public async Task<bool> StartClientWithRelay(string joinCode)
    {
        try {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException ex)
        {
            Debug.Log(ex.Message);
            return false;
        }
    }
}
