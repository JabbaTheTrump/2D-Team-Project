using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProceduralRoomGenerator : MonoBehaviour
{
    public Vector2 MapCenterPosition;
    public Vector2 MapSize;
    public float WallThickness = 1;

    //public PRG_SpawnableRoomEntry[] SpawnableRoomTypes;

    public void GenerateMap()
    {
        Vector2 testRoomSize = new(3, 4);
        Vector2 testRoomTopLPosition = new(
            MapCenterPosition.x - 0.5f * testRoomSize.x,
            MapCenterPosition.y + 0.5f * testRoomSize.y 
            );

        PRG_Room currentRoom = new(testRoomTopLPosition, testRoomSize);


    }

    //void GenerateRoom
}

public class PRG_Room
{
    public Vector2 TopLeftPosition;
    public Vector2 Size;
    //PRG_RoomTemplate Template;

    public PRG_Room(Vector2 topLeftPos, Vector2 size)
    {
        TopLeftPosition = topLeftPos;
        Size = size;
    }
}

//public class PRG_SpawnableRoomEntry
//{
//    public PRG_RoomTemplate RoomTemplate;
//    public float SpawnWeight;
//    public int MinRoomCount;
//    public int MaxRoomCount;
//}
