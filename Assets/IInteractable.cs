using Unity.Netcode;
using UnityEngine;

public interface IInteractable
{
    public void Interact(NetworkObject interactor);
}

public interface IProgressBarInteractable
{
    public void Interact(NetworkObject interactor);
}