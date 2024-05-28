using Unity.VisualScripting;
using UnityEditor.TerrainTools;
using UnityEngine;

[CreateAssetMenu(menuName = "PRG/Room Template")]
public class PRG_RoomTemplate : ScriptableObject
{
    [HideInInspector] public Vector2 RoomFloorSize { get; private set; }

    public GameObject RoomObject;

    private void Awake()
    {
        RoomFloorSize = RoomObject.transform.localScale;
    }
}
