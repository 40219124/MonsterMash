using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ERoomState { NotSeen, Seen, Completed };

[Serializable]
public class MapRoom
{
    public Room RoomData;

    public ERoomState RoomState;
	public bool IsStartingRoom = false;

    public Vector2Int MapPosition;

    public Room.eDoorPlaces UsedDoorPlaces;

	public MapRoom(Room roomData, Room.eDoorPlaces usedDoorPlaces, Vector2Int mapPosition)
	{
		RoomData = roomData;
		UsedDoorPlaces = usedDoorPlaces;
		MapPosition = mapPosition;
	}

	public bool PositionIsDoor(int index)
    {
		return (index ==  Room.DoorPlaceTop1 ||
				index == Room.DoorPlaceTop2) &&
				UsedDoorPlaces.HasFlag(Room.eDoorPlaces.Top) ||

				(index ==  Room.DoorPlaceBottom1 ||
				index == Room.DoorPlaceBottom2) &&
				UsedDoorPlaces.HasFlag(Room.eDoorPlaces.Bottom) ||

				(index ==  Room.DoorPlaceLeft1 ||
				index == Room.DoorPlaceLeft2) &&
				UsedDoorPlaces.HasFlag(Room.eDoorPlaces.Left) ||

				(index ==  Room.DoorPlaceRight1 ||
				index == Room.DoorPlaceRight2) &&
				UsedDoorPlaces.HasFlag(Room.eDoorPlaces.Right);
    }
}
