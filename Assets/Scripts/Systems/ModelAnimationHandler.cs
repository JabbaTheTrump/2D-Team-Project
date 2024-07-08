using Animancer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelAnimationHandler : MonoBehaviour
{
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetAnimationTrigger(string animationTriggerName)
    {
        _animator.SetTrigger(animationTriggerName);
    }
}
