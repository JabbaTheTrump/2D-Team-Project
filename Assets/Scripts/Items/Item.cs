using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Tutorials.Core.Editor;
using UnityEngine;

[System.Serializable]
public abstract class Item : NetworkBehaviour
{
    public ItemData ItemData;
    public abstract void Use();
}
