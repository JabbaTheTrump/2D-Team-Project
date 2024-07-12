using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class InventorySystem : NetworkBehaviour
{
    [Header("Properties")]
    [SerializeField] public readonly int SlotCount = 4;

    [Header("Exposed Fields")]
    public InventorySlot[] InventorySlots;
    [field: SerializeField] public int SelectedSlot { get; private set; } = 0;

    [Header("Serialized References")]
    [SerializeField] GameObject _droppedItemPrefab;

    //Events
    public event Action<int> OnItemPickedUp;
    public event Action OnItemRemoved;
    public event Action<int> OnNewSlotSelected;

    private void Start()
    {
        InitializeInventorySlots();
    }

    //public override void OnNetworkSpawn()
    //{
    //    base.OnNetworkSpawn();
    //    enabled = IsServer || IsOwner;
    //}

    private void InitializeInventorySlots()
    {
        InventorySlots = new InventorySlot[SlotCount];

        for (int i = 0; i < SlotCount; i++)
        {
            InventorySlots[i] = new InventorySlot(i);
        }
    }


    public bool TryPickUpItem(Item item)
    {
        if (item == null) return false; //Returns if the item is null

        InventorySlot firstEmptySlot = InventorySlots.Where(slot => slot.Item == null).FirstOrDefault(); //Retrieves the first empty slot in the inventory
        if (firstEmptySlot == null) return false; //Returns if no empty slot was found

        if (firstEmptySlot.AddItem(item))
        {
            OnItemPickedUp?.Invoke(firstEmptySlot.Index);
            SyncItemAddedClientRpc(firstEmptySlot.Index, item.ItemData.Id);
            return true;
        }
        return false;
    }

    public InventorySlot GetSlotByItemData(ItemData itemData) //Returns the first slot with an identical item data
    {
        return InventorySlots.Where(slot => slot.Item != null && slot.Item.ItemData == itemData).FirstOrDefault();
    }

    public InventorySlot GetSlotByItemType<T>() where T : ItemData
    {
        InventorySlot slot = InventorySlots.
            Where(slot => slot.Item != null && typeof(T) == (slot.Item.ItemData.GetType())).FirstOrDefault();
        Debug.Log(slot);
        return slot;
    }

    public bool TryRemoveItem(int slotIndex, bool dropItem)
    {
        if (slotIndex < 0 || slotIndex >= InventorySlots.Length) return false; //Returns false if the slot index is invalid
        if (InventorySlots[slotIndex].Item == null) return false; //Returns false if the slot is empty

        Debug.Log($"Dropping {InventorySlots[slotIndex].Item}");


        if (dropItem) //Drops the item on the server if needed
        {
            SpawnDroppedItem(slotIndex);
        }

        Item itemCopy = InventorySlots[slotIndex].Item;

        InventorySlots[slotIndex].RemoveItem(); //Empties the slot

        OnItemRemoved?.Invoke();
        SyncItemRemovedClientRpc(slotIndex);

        return true;
    }

    void SpawnDroppedItem(int slotIndex)
    {
        NetworkObject droppedItemObject = NetworkSpawnManager.Instance.SpawnPrefabOnNetwork(_droppedItemPrefab, transform.position);

        if (droppedItemObject == null)
        {
            Debug.LogWarning($"Dropped item in slot {slotIndex} failed to spawn!");
        }
        else
        {
            droppedItemObject.GetComponent<DroppedItem>().SetItem(InventorySlots[slotIndex].Item); //Updates the dropped item's data
        }
    }

    public void SelectSlot(int slotIndex)
    {
        if (SelectedSlot == slotIndex) return;

        SelectedSlot = slotIndex;
        OnNewSlotSelected?.Invoke(slotIndex);
    }

    [ClientRpc]
    void SyncItemAddedClientRpc(int slotIndex, int itemId)
    {
        if (IsServer) return;

        Item item = ItemDictionary.Instance.GetItemById(itemId);

        if (item == null)
        {
            Debug.LogWarning($"A client has not registered an item with it's ID correctly! ID:{itemId}");
            return;
        }

        InventorySlots[slotIndex].Item = item;
        OnItemPickedUp?.Invoke(slotIndex);
    }

    [ClientRpc]
    void SyncItemRemovedClientRpc(int slotIndex)
    {
        if (IsServer) return;

        InventorySlots[slotIndex].RemoveItem();
        OnItemRemoved?.Invoke();
    }

}
