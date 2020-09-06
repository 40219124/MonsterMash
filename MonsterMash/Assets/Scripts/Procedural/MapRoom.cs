using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ERoomState { NotSeen, Seen, Completed };

public enum ERoomDecoration { None = -1, Potion1, Potions2, Flowers, BrownGrass1, BrownGrass2, BrownGrass3 }

[Serializable]
public class MapRoom
{
    public Room RoomData;

    public ERoomState RoomState;
    public bool IsStartingRoom = false;
	public bool IsBossRoom {get; private set;}

    public Vector2Int MapPosition;

    public Room.eDoorPlaces UsedDoorPlaces;

    public ERoomDecoration[,] RoomDecorations = new ERoomDecoration[Room.GameWidth, Room.GameHeight];

	public List<CollectableItem> CollectableItems = new List<CollectableItem>();

	public void SetAsBossRoom()
	{
		if(!IsBossRoom)
		{
			IsBossRoom = true;
			var bossPrize = new BossPrize();

			CollectableItems.Add(bossPrize);
		}
	}

	public MapRoom(Room roomData, Room.eDoorPlaces usedDoorPlaces, Vector2Int mapPosition)
	{
		RoomData = roomData;
		UsedDoorPlaces = usedDoorPlaces;
		MapPosition = mapPosition;

		if(UnityEngine.Random.Range(0f, 100f) <= Settings.ChanceOfHealingPotion)
		{
			var healingPotion = new HealingPotion();

			CollectableItems.Add(healingPotion);
		}
	}

    public bool PositionIsDoor(int index)
    {
        return (index == Room.DoorPlaceTop1 ||
                index == Room.DoorPlaceTop2) &&
                UsedDoorPlaces.HasFlag(Room.eDoorPlaces.Top) ||

                (index == Room.DoorPlaceBottom1 ||
                index == Room.DoorPlaceBottom2) &&
                UsedDoorPlaces.HasFlag(Room.eDoorPlaces.Bottom) ||

                (index == Room.DoorPlaceLeft1 ||
                index == Room.DoorPlaceLeft2) &&
                UsedDoorPlaces.HasFlag(Room.eDoorPlaces.Left) ||

                (index == Room.DoorPlaceRight1 ||
                index == Room.DoorPlaceRight2) &&
                UsedDoorPlaces.HasFlag(Room.eDoorPlaces.Right);
    }
}
