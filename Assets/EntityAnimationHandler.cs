using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EntityAnimationHandler : ServerSideNetworkedBehaviour
{
    Animator _animator;
    AIMovement _movementHandler;

    [Header("Animation Clips")]

    [SerializeField] AnimationClip _attackClip;

    //[SerializeField] float _fadeDuration = 0.5f;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        _animator = GetComponentInChildren<Animator>();
        _movementHandler = GetComponent<AIMovement>();

        _movementHandler.CurrentMovementState.OnValueChanged += MovementStateChanged; //Updates the animation whenever the entity switches movement type
    }

    void MovementStateChanged(MovementState state)
    {
        _animator.SetTrigger(_movementHandler.GetMovementTypeByState(state).AnimationTriggerName);
    }

    public void PlayAttackAnimation()
    {
        _animator.SetTrigger("Attack");
    }
}
