using UnityEngine;

[CreateAssetMenu(menuName = "ItemData")]
public class ItemData : ScriptableObject
{
    [field:SerializeField] public int Id { get; private set; }
    public string Name;
    public string Description;
    public Sprite ItemSprite;

    public void SetId(int id)
    {
        Id = id;
    }
}