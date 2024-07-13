using Unity.Netcode;
using UnityEngine;

public interface IInteractable
{
    public NetworkVariable<bool> IsInteractable { get; set; }
    public void Interact(NetworkObject interactor);
}

public interface IToggleableInteractable : IInteractable
{
    public NetworkVariable<bool> ToggledOn { get; set; }
}

public abstract class ProgressBarInteractableObject : NetworkBehaviour, IInteractable
{
    public NetworkVariable<bool> IsInteractable { get; set; }
    public NetworkVariable<bool> BeingInteractedWith { get; set; } = new(false);
    [field:SerializeField] public float InteractionTime { get; private set; } = 1f;

    public abstract void Interact(NetworkObject interactor);
}