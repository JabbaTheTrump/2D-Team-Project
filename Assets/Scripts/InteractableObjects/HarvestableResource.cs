using Unity.Netcode;
using UnityEngine;

public class HarvestableResource : NetworkBehaviour, IInteractable
{
    [field:SerializeField] public ResourceData Data { get; private set; }
    public NetworkVariable<bool> IsInteractable { get; set; } = new(true);

    public void Interact(NetworkObject interactor)
    {
        InventorySystem inventorySystem = interactor.GetComponentInChildren<InventorySystem>();

        if (inventorySystem == null) return;

    }
}
