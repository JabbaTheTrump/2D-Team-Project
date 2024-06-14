using System;
using Unity.Netcode;
using UnityEngine;

class AINoiseSensor : NetworkBehaviour
{
    [Header("Properties")]
    [SerializeField] WalkerAI _aiHandler;
    [SerializeField] float _maxNoiseDistance = 0;

    [Header("Debug")]
    [SerializeField] float _distanceFromAssignedNoise = 0;
    [field: SerializeField] public Vector2 AssignedNoisePoint { get; private set; }
    public event Action<Vector2> OnCloserNoiseDetected;

    public void RegisterNoise(Vector2 position)
    {
        float distanceToNoise = Vector2.Distance(position, transform.position);

        if (distanceToNoise > _maxNoiseDistance) return;

        _distanceFromAssignedNoise = Vector2.Distance(transform.position, AssignedNoisePoint);

        if (AssignedNoisePoint == Vector2.zero || _distanceFromAssignedNoise >= distanceToNoise) //If no noise was recently detected OR a closer noise was detected
        {
            Debug.Log("Noise registered proprely!");
            AssignNoise(position, distanceToNoise);
        }
    }

    void AssignNoise(Vector2 position, float distance)
    {
        AssignedNoisePoint = position;
        _distanceFromAssignedNoise = distance;

        OnCloserNoiseDetected?.Invoke(position);
        _aiHandler.movementHandler.OnReachedPoint += ClearAssignedNoise;
    }

    public void ClearAssignedNoise()
    {
        _aiHandler.movementHandler.OnReachedPoint -= ClearAssignedNoise;
        AssignNoise(Vector2.zero, 0);
    }
}