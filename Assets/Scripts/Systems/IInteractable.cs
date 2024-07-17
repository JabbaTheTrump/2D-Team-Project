using Unity.Netcode;
using UnityEngine;

public interface IInteractable
{
    public NetworkVariable<bool> IsInteractable { get; set; }
    //public NetworkVariable<bool> BeingInteractedWith { get; set; }
    public float InteractionTime { get; set; }

    public void Interact(NetworkObject interactor);
}

public interface IToggleableInteractable : IInteractable
{
    public NetworkVariable<bool> ToggledOn { get; set; }
}
