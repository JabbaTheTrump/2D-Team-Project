using UnityEngine;

[CreateAssetMenu(menuName = "ItemData/Generic")]
public class ItemData : ScriptableObject
{
    public int Id { get; private set; }
    public string Name;
    public string Description;
    public Sprite ItemSprite;
    public Sprite ItemIconUI;

    public void SetId(int id)
    {
        Id = id;
    }
}