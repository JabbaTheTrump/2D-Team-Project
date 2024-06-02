using Unity.Netcode;
using UnityEngine;

public interface IInteractable
{
    public NetworkVariable<bool> IsInteractable { get; set; }
    public void Interact(NetworkObject interactor);
}

public interface IProgressBarInteractable
{
    public void Interact(NetworkObject interactor);
}