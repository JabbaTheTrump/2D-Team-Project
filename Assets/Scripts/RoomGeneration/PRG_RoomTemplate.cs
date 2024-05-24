using Unity.VisualScripting;
using UnityEditor.TerrainTools;
using UnityEngine;

[CreateAssetMenu(menuName = "PRG/Room Template")]
public class PRG_RoomTemplate : ScriptableObject
{
    [HideInInspector] public Vector2 RoomSize { get; private set; }
    public float WallThickness = 1;

    public GameObject RoomObject;

    private void Awake()
    {
        RoomSize = RoomObject.transform.localScale;
    }
}
