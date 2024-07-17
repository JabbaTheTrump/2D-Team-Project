using Unity.Netcode;
using UnityEngine;

public class HarvestableResource : NetworkBehaviour, IInteractable
{
    [field:SerializeField] public ResourceData Data { get; private set; }
    public NetworkVariable<bool> IsInteractable { get; set; } = new(true);
        public NetworkVariable<bool> BeingInteractedWith { get; set; } = new(false);
    public float InteractionTime { get; set; }

    public void Interact(NetworkObject interactor)
    {
        InventorySystem inventorySystem = interactor.GetComponentInChildren<InventorySystem>();

        if (inventorySystem == null) return;

    }
}
