using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GameStateController : NetworkSingleton<GameStateController>
{
    [Header("Game Properties")]
    public NetworkVariable<int> RequiredSampleCount = new(5);


    public NetworkVariable<int> CurrentSampleCount { get; private set; } = new(0);


    public void AddSample()
    {
        CurrentSampleCount.Value++;

        if (CurrentSampleCount.Value >= RequiredSampleCount.Value)
        {
            CompletedObjective();
        }
    }

    void CompletedObjective()
    {

    }
}