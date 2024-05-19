using Unity.Netcode;
using UnityEngine;

public class HarvestableResource : NetworkBehaviour, IInteractable
{
    [field:SerializeField] public ResourceData Data { get; private set; }

    public void Interact()
    {
        Debug.Log("Yes");
    }
}
