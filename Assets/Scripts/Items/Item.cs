using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;

public abstract class Item : NetworkBehaviour
{
    public ItemData ItemData { get; set; }
    public abstract void Use();
}
