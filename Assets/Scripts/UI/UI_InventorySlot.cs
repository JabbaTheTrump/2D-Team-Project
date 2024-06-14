using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_InventorySlot : MonoBehaviour
{
    [SerializeField] Image _iconRenderer;

    public void UpdateSlot(Sprite icon)
    {
        _iconRenderer.sprite = icon;
    }
}
