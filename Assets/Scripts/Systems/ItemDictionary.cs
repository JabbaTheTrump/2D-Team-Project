using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemDictionary : Singleton<ItemDictionary>
{
    [SerializeField] List<Item> _inGameItems;
    Dictionary<int, Item> _itemIdDictionary = new Dictionary<int, Item>();

    // Start is called before the first frame update
    void Start()
    {
        AssignIds();
    }

    void AssignIds()
    {
        foreach (var item in _inGameItems)
        {
            int id = _itemIdDictionary.Count; //Sets the ID to the current amount of items in the dictionary
            _itemIdDictionary.Add(id, item);
            item.ItemData.SetId(id);
        }
    }

    public Item GetItemById(int id)
    {
        return _itemIdDictionary[id];
    }
}
