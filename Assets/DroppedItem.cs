using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DroppedItem : NetworkBehaviour, IInteractable
{
    //Fields
    [SerializeField] Item _item;

    //Serialized References
    [SerializeField] SpriteRenderer _spriteRenderer;

    public NetworkVariable<bool> IsInteractable { get; set; } = new(true);

    public void SetItem(Item item)
    {
        _item = item;
        gameObject.name = $"DroppedItem[{_item.ItemData.Name}]";
        
        if (_item.ItemData.ItemSprite != null )
        {
            _spriteRenderer.sprite = _item.ItemData.ItemSprite;
        }
    }

    public void Interact(NetworkObject interactor)
    {
        InventorySystem inventorySystem = interactor.GetComponentInChildren<InventorySystem>();
        if (inventorySystem == null)
        {
            Debug.LogWarning($"An entity [{interactor}] without an inventory system had attempted to interact with a dropped item [{transform.root}]");
            return;
        }

        if (!inventorySystem.TryPickUpItem(_item)) //Tries to pick up the item and exists method if attempt failed
        {
            return;
        }

        NetworkSpawnManager.Instance.DespawnObjectOnNetwork(GetComponent<NetworkObject>());
    }
}
