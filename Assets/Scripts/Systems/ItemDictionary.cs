using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ItemDictionary : Singleton<ItemDictionary>
{
    [SerializeField] List<Item> _itemList;


    private void Start()
    {
        LoadAllItemData();
        AssignIds();
    }

    void LoadAllItemData() //Loads all the items into a list from the client
    {
        List<Item> itemDataList = Resources.LoadAll<Item>("Items").ToList();
        //Debug.Log($"Item Dictionary has loaded a total of {itemDataList.Count} items!");

        _itemList = itemDataList;
    }

    void AssignIds() //Pairs IDs with Items on the server
    {
        for (int i = 0; i < _itemList.Count; i++)
        {
            _itemList[i].ItemData.SetId(i);
            //Debug.Log($"Paired ID: {i}, {_itemDataList[i]}");
        }
    }

    public Item GetItemById(int id)
    {
        return _itemList
            .Where(item => item.ItemData.Id == id)
            .SingleOrDefault();
    }

    public ItemData GetItemDataById(int id)
    {
        return _itemList
            .Where(item => item.ItemData.Id == id)
            .SingleOrDefault().ItemData;
    }
}
