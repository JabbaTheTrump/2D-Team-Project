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
        public ItemData ItemData;
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


    public bool TryPickUpItem(ItemData item)
    {
        if (item == null) return false; //Returns if the item is null

        InventorySlot firstEmptySlot = InventorySlots.Where(slot => slot.ItemData == null).FirstOrDefault(); //Retrieves the first empty slot in the inventory
        if (firstEmptySlot == null) return false; //Returns if no empty slot was found

        return firstEmptySlot.AddItem(item); //Returns whether the item was successfuly added
    }

    public InventorySlot GetSlotByItemData(ItemData itemData) //Returns the first slot with an identical item data
    {
        return InventorySlots.Where(slot => slot.ItemData == itemData).FirstOrDefault();
    }

    public InventorySlot GetSlotByItemType<T>()
    {
        return InventorySlots.Where(slot => typeof(T) == slot.ItemData.GetType()).FirstOrDefault();
    }

    public bool TryRemoveItem(int index, bool dropItem)
    {
        if (index < 0 || index >= InventorySlots.Length) return false; //Returns false if the slot index is invalid
        if (InventorySlots[index].ItemData == null) return false; //Returns false if the slot is empty

        Debug.Log($"Dropping {InventorySlots[index].ItemData}");

        OnItemRemoved?.Invoke(new OnItemRemovedEventArguements
        {
            ItemData = InventorySlots[index].ItemData,
            DropItem = dropItem
        });

        if (dropItem) //Drops the item on the server if needed
        {
            NetworkObject droppedItemObject = NetworkSpawnManager.Instance.SpawnPrefabOnNetwork(_droppedItemPrefab, transform.position);

            if (droppedItemObject == null)
            {
                Debug.LogWarning($"Dropped item in slot {index} failed to spawn!");
            }
            else
            {
                droppedItemObject.GetComponent<DroppedItem>().SetData(InventorySlots[index].ItemData); //Updates the dropped item's data
            }
        }

        InventorySlots[index].RemoveItem(); //Empties the slot

        return true;
    }
}
