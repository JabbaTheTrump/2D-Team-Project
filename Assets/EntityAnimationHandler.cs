using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EntityAnimationHandler : NetworkBehaviour
{
    ModelAnimationHandler _modelAnimationHandler;
    AIMovement _movementHandler;

    [Header("Animation Clips")]
    [SerializeField] AnimationClip _idleClip;
    [SerializeField] AnimationClip _walkClip;
    [SerializeField] AnimationClip _chaseClip;
    [SerializeField] AnimationClip _attackClip;

    [SerializeField] float _fadeDuration = 0.5f;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        enabled = IsServer;

        _modelAnimationHandler = GetComponentInChildren<ModelAnimationHandler>();
        _movementHandler = GetComponent<AIMovement>();

        _movementHandler.IsWalking.OnValueChanged += (_, __) => MovementStateChanged();
        _movementHandler.IsChasing.OnValueChanged += (_, __) => MovementStateChanged();
    }

    void MovementStateChanged()
    {
        if (_movementHandler.IsWalking.Value)
        {
            _modelAnimationHandler.PlayAnimation(_walkClip, _fadeDuration);
        }
        else if (_movementHandler.IsChasing.Value)
        {
            _modelAnimationHandler.PlayAnimation(_chaseClip, _fadeDuration);
        }
    }
}
