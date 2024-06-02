using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class MonitorDisplayManager : NetworkBehaviour //Sets the in-game display to show the sample collection progress
{
    //Serialized Fields
    [SerializeField] TextMeshPro sampleCountDisplay;

    // Start is called before the first frame update
    void Start()
    {
        PlayerSpawnController.OnLocalPlayerSpawn += (_) => Initialize();
    }

    private void Initialize()
    {
        SetMonitor();
        GameStateController.Instance.CurrentSampleCount.OnValueChanged += (_, __) => SetMonitor();
    }

    void SetMonitor()
    {
        sampleCountDisplay.text = $"{GameStateController.Instance.CurrentSampleCount.Value}/{GameStateController.Instance.RequiredSampleCount.Value}";
    }
}
