using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemDictionary : Singleton<ItemDictionary>
{
    [SerializeField] List<GameObject> _inGameItems;
    Dictionary<int, ItemData> _itemIdDictionary = new Dictionary<int, ItemData>();

    // Start is called before the first frame update
    void Start()
    {
        AssignIds();
    }

    void AssignIds()
    {
        foreach (var itemObject in _inGameItems)
        {
            IItem item = itemObject.GetComponent<IItem>();

            if (item == null)
            {
                Debug.LogWarning($"Item dictionary has attempted to assign an ID to an object without an Item script! ({itemObject})");
                continue;
            }

            int id = _itemIdDictionary.Count; //Sets the ID to the current amount of items in the dictionary
            _itemIdDictionary.Add(id, item.ItemData);
            item.ItemData.SetId(id);
        }
    }

    public ItemData GetItemDataById(int id)
    {
        return _itemIdDictionary[id];
    }
}
