using JetBrains.Annotations;
using Unity.VisualScripting;

[System.Serializable]
public class InventorySlot
{
    public Item Item;
    public int Index;

    public InventorySlot(int index)
    {
        Index = index;
    }

    public bool AddItem(Item item) //Changes the item data to the given data, returns whether the change was successful
    {
        if (Item != null) return false;

        Item = item;
        return true;
    }

    public bool RemoveItem() //Removes the item data, returns whether there was an item to remove
    {
        if (Item == null) return false;

        Item = null;
        return true;
    }
}