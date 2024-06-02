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
    [SerializeField] int SlotCount = 4;

    [Header("Exposed Fields")]
    public InventorySlot[] InventorySlots;
    [field: SerializeField] public int SelectedSlot { get; private set; } = 0;

    [Header("Serialized References")]
    [SerializeField] GameObject _droppedItemPrefab;

    //Events
    public event Action<int> OnItemPickedUp;
    public event Action<OnItemRemovedEventArguements> OnItemRemoved;

    //Event arguements
    public class OnItemRemovedEventArguements
    {
        public Item Item;
        public bool DropItem = false;
    }

    private void Start()
    {
        InitializeInventorySlots();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        enabled = IsServer;
    }

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

    public bool TryRemoveItem(int index, bool dropItem)
    {
        if (index < 0 || index >= InventorySlots.Length) return false; //Returns false if the slot index is invalid
        if (InventorySlots[index].Item == null) return false; //Returns false if the slot is empty

        Debug.Log($"Dropping {InventorySlots[index].Item}");


        if (dropItem) //Drops the item on the server if needed
        {
            NetworkObject droppedItemObject = NetworkSpawnManager.Instance.SpawnPrefabOnNetwork(_droppedItemPrefab, transform.position);

            if (droppedItemObject == null)
            {
                Debug.LogWarning($"Dropped item in slot {index} failed to spawn!");
            }
            else
            {
                droppedItemObject.GetComponent<DroppedItem>().SetItem(InventorySlots[index].Item); //Updates the dropped item's data
            }
        }

        Item itemCopy = InventorySlots[index].Item;

        InventorySlots[index].RemoveItem(); //Empties the slot

        OnItemRemoved?.Invoke(new OnItemRemovedEventArguements
        {
            Item = itemCopy,
            DropItem = dropItem
        });

        return true;
    }
}
