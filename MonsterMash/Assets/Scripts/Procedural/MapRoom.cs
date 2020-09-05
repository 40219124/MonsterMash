using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRoom
{
    public Room RoomData;

    public bool IsCompleted = false;

    public Vector2Int MapPosition;

    public Room.eDoorPlaces UsedDoorPlaces;
}
