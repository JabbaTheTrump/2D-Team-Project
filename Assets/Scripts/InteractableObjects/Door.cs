using Animancer;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.AI;

public class Door : NetworkBehaviour, IToggleableInteractable
{
    public NetworkVariable<bool> IsInteractable { get; set; } = new(true);
    public NetworkVariable<bool> ToggledOn { get; set; } = new(false);

    private Animator _animator;
    private NavMeshObstacle _obstacle;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Interact(NetworkObject interactor)
    {
        ToggledOn.Value = !ToggledOn.Value; //Flips the door's state

        if (ToggledOn.Value) OpenDoor(); //Determines the type of interaction based on the updated state
        else CloseDoor();
    }

    void OpenDoor()
    {
        _animator.SetBool("IsOpen", true);
    }

    void CloseDoor()
    {
        _animator.SetBool("IsOpen", false);
    }
}
