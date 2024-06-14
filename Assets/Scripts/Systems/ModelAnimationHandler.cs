using Animancer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelAnimationHandler : MonoBehaviour
{
    [SerializeField] AnimancerComponent _animancer;

    private void Start()
    {
        _animancer = GetComponent<AnimancerComponent>();
    }

    public void PlayAnimation(AnimationClip clip)
    {
        _animancer.Play(clip);
    }

    public void PlayAnimation(AnimationClip clip, float transitionDuration)
    {
        _animancer.Play(clip, transitionDuration);
    }
}
