using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProceduralRoomGenerator : MonoBehaviour
{
    public Vector2 TopLeftPosition;
    public Vector2 MapSize;

    public PRG_SpawnableRoomEntry[] SpawnableRoomTypes;

    public void GenerateMap()
    {
        
    }
}

public class PRG_SpawnableRoomEntry
{
    public PRG_RoomTemplate RoomTemplate;
    public float SpawnWeight;
    public int MinRoomCount;
    public int MaxRoomCount;
}

public class PRG_Room
{
    public Vector2 Position;
    public Vector2 Size;
}
