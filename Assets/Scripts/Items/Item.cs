using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;

public interface IItem
{
    public ItemData ItemData { get; set; }
    public void Use();
}
