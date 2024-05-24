using JetBrains.Annotations;
using Unity.VisualScripting;

[System.Serializable]
public class InventorySlot
{
    public ItemData ItemData;

    public bool AddItem(ItemData item) //Changes the item data to the given data, returns whether the change was successful
    {
        if (ItemData != null) return false;

        ItemData = item;
        return true;
    }

    public bool RemoveItem() //Removes the item data, returns whether there was an item to remove
    {
        if (ItemData == null) return false;

        ItemData = null;
        return true;
    }
}