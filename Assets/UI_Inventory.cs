using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class UI_Inventory : MonoBehaviour
{
    InventorySystem _inventory;

    [SerializeField] List<UI_InventorySlot> _uiSlots;

    [SerializeField] GameObject _slotPrefab;
    [SerializeField] Sprite _emptyIcon;

    void Start()
    {
        PlayerSpawnController.OnLocalPlayerSpawn += InitializeInventoryUI;
    }

    void InitializeInventoryUI(GameObject playerObject)
    {
        _inventory = playerObject.GetComponentInChildren<InventorySystem>();

        if (_inventory == null)
        {
            Debug.Log("Attempting to generate slots with a null inventory reference");
            return;
        }

        _inventory.OnItemRemoved += (_) => UpdateInventoryDisplay();
        _inventory.OnItemPickedUp += (_) => UpdateInventoryDisplay();

        GenerateInventorySlots();
        UpdateInventoryDisplay();
    }

    void GenerateInventorySlots() //Creates a UI slot for each inventory slot 
    {
        Debug.Log(_inventory.InventorySlots.Length);
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        _uiSlots = new();

        foreach (var invSlot in _inventory.InventorySlots)
        {
            _uiSlots.Add(Instantiate(_slotPrefab, transform).GetComponent<UI_InventorySlot>());
        }
    }

    void UpdateInventoryDisplay()
    {
        for (int i = 0; i < _uiSlots.Count; i++)
        {
            Item slotItem = _inventory.InventorySlots[i].Item;

            if (slotItem == null)
            {
                _uiSlots[i].UpdateSlot(_emptyIcon);
                return;
            }

            _uiSlots[i].UpdateSlot(slotItem.ItemData.ItemIconUI);
        }
    }
}
