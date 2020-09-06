using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapController : MonoBehaviour
{
    [SerializeField] List<Animator> RoomAnimators;


	void Update()
	{
		UpdateMap();
	}
	
	void UpdateMap()
	{
		var map = ProceduralDungeon.Instance.DungeonMap;
		var currentRoom = ProceduralDungeon.Instance.CurrentRoom;

		int index = 0;
		for (int x = 0; x < Settings.MapSize; x++)
		{
			for (int y = 0; y < Settings.MapSize; y++)
			{
				int roomState = 0;

				var room = map[y, x];

				if(currentRoom == new Vector2Int(y, x))
				{
					roomState = 1;
				}
				else if(room != null)
				{
					if (room.RoomState == ERoomState.Completed)
					{
						roomState = 2;
					}
					else if(RoomNearSeen(x, y, map))
					{
						roomState = 3;
					}
				}

				if (index >= RoomAnimators.Count)
				{
					Debug.LogError("not enough RoomAnimators set up for set up for mini map");
				}
				else
				{
					RoomAnimators[index].SetInteger("RoomState", roomState);
				}
				index += 1;
			}
		}
	}

	bool RoomNearSeen(int x, int y, MapRoom[,] map)
	{
		if (x < Settings.MapSize-1 && 
			map[y, x+1] != null &&
			map[y, x+1].RoomState != ERoomState.NotSeen)
		{
			return true;
		}
		if (x > 0 && 
			map[y, x-1] != null &&
			map[y, x-1].RoomState != ERoomState.NotSeen)
		{
			return true;
		}
		if (y < Settings.MapSize-1 && 
			map[y+1, x] != null &&
			map[y+1, x].RoomState != ERoomState.NotSeen)
		{
			return true;
		}
		if (y > 0 && 
			map[y-1, x] != null &&
			map[y-1, x].RoomState != ERoomState.NotSeen)
		{
			return true;
		}

		return false;
	}
}
