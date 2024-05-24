using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DroppedItem : NetworkBehaviour, IInteractable
{
    //Fields
    [SerializeField] ItemData _data;

    //Serialized References
    [SerializeField] SpriteRenderer _spriteRenderer;

    public void SetData(ItemData data)
    {
        gameObject.name = $"DroppedItem[{_data.Name}]";
        _data = data;
        
        if (_data.ItemSprite != null )
        {
            _spriteRenderer.sprite = _data.ItemSprite;
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

        if (!inventorySystem.TryPickUpItem(_data)) //Tries to pick up the item and exists method if attempt failed
        {
            return;
        }

        NetworkSpawnManager.Instance.DespawnObjectOnNetwork(GetComponent<NetworkObject>());
    }
}
