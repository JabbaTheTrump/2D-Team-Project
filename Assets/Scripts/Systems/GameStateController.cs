using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GameStateController : ServerSingleton<GameStateController>
{
    [Header("Game Properties")]
    public NetworkVariable<int> RequiredSampleCount = new(5);


    public NetworkVariable<int> CurrentSampleCount { get; private set; } = new(0);


    //Events
    public event Action<GameOverEventArgs> OnGameOver;

    public struct GameOverEventArgs
    {
        public bool IsVictory;
    }

    private void Start()
    {
        PlayerStateManager.Instance.OnPlayerStateListChange += CheckPlayersState;
    }

    public void AddSample()
    {
        CurrentSampleCount.Value++;

        if (CurrentSampleCount.Value >= RequiredSampleCount.Value)
        {
            CompletedObjective();
        }
    }

    void CheckPlayersState(List<PlayerStateManager.PlayerStateEntry> playerStateList)
    {
        foreach (var entry in playerStateList)
        {
            if (entry.State == PlayerStateManager.PlayerState.Spawned_Alive) return;
        }

        GameOverEventArgs eArgs = new()
        {
            IsVictory = false
        };

        OnGameOver?.Invoke(eArgs);
    }

    void CompletedObjective()
    {
        GameOverEventArgs eArgs = new()
        { 
            IsVictory = true 
        };

        OnGameOver?.Invoke(eArgs);
    }
}