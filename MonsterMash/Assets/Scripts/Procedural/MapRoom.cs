using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapRoom
{
    public Room RoomData;

    public bool IsCompleted = false;
	public bool IsStartingRoom = false;

    public Vector2Int MapPosition;

    public Room.eDoorPlaces UsedDoorPlaces;

	public MapRoom(Room roomData, Room.eDoorPlaces usedDoorPlaces, Vector2Int mapPosition)
	{
		RoomData = roomData;
		UsedDoorPlaces = usedDoorPlaces;
		MapPosition = mapPosition;
	}
}
