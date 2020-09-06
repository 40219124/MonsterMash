using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ProceduralDungeon : MonoBehaviour
{
	public static ProceduralDungeon Instance;

	public MapRoom[,] DungeonMap = new MapRoom[Settings.MapSize, Settings.MapSize];
	public Vector2Int StartingRoom;
	public Vector2Int CurrentRoom;
	public AllRoomData AllRoomsData;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			GenerateMap();
		}
	}

	public MapRoom GetCurrentRoom()
	{
		return DungeonMap[CurrentRoom.x,CurrentRoom.y];
	}

	void GenerateMap()
	{
		string path = Application.streamingAssetsPath + "/RoomData.json";
		var roomData = File.ReadAllText(path);
		AllRoomsData = JsonUtility.FromJson<AllRoomData>(roomData);

		StartingRoom = new Vector2Int(Random.Range(0, Settings.MapSize), Random.Range(0, Settings.MapSize));
		CurrentRoom = StartingRoom;
		var roughMap = MakeRoughMap(StartingRoom);
		Debug.Log($"Built roughMap: {ArrayToString(roughMap)}");

		BuildRealMap(roughMap, Room.eArea.Outdoors);

		Debug.Log($"Built real map: {ArrayToString(DungeonMap)}");
	}

	void BuildRealMap(bool[,] roughMap, Room.eArea area)
	{
		for (int x = 0; x < Settings.MapSize; x++)
		{
			for (int y = 0; y < Settings.MapSize; y++)
			{
				MapRoom mapRoom = null;
				if (roughMap[x, y])
				{
					var pos = new Vector2Int(x, y);
					var neededDoorPos = GetRequiredDoorPostions(roughMap, pos);
					var validRooms = AllRoomsData.ValidRooms(neededDoorPos, area);

					if (validRooms.Count > 0)
					{
						var roomData = validRooms[Random.Range(0, validRooms.Count)];
						mapRoom = new MapRoom(roomData, neededDoorPos, pos);
					}
					else
					{
						Debug.LogError($"AllRoomsData cannot find room {neededDoorPos} from area: {area}");
					}
				}
				DungeonMap[x,y] = mapRoom;
			}
		}

		DungeonMap[StartingRoom.x,StartingRoom.y].IsStartingRoom = true;
	}

	Room.eDoorPlaces GetRequiredDoorPostions(bool[,] roughMap, Vector2Int pos)
	{
		var neededDoors = Room.eDoorPlaces.None;

		if(pos.y+1 < Settings.MapSize && roughMap[pos.x, pos.y+1])
		{
			neededDoors &= Room.eDoorPlaces.Top;
		}

		if(pos.y-1 >= 0 && roughMap[pos.x, pos.y-1])
		{
			neededDoors &= Room.eDoorPlaces.Bottom;
		}

		if(pos.x+1 < Settings.MapSize && roughMap[pos.x+1, pos.y])
		{
			neededDoors &= Room.eDoorPlaces.Right;
		}

		if(pos.x-1 >= 0 && roughMap[pos.x-1, pos.y])
		{
			neededDoors &= Room.eDoorPlaces.Left;
		}
		return neededDoors;
	}

	bool[,] MakeRoughMap(Vector2Int startPos, bool[,] roughMap=null, int numberRoomsMade=1)
	{
		if (roughMap == null)
		{
			roughMap = new bool[Settings.MapSize, Settings.MapSize];
		}


		var roomTodoList = new Queue<Vector2Int>();
		roomTodoList.Enqueue(startPos);

		while (roomTodoList.Count != 0 && numberRoomsMade < Settings.MaxRooms)
		{
			var currentRoomPos = roomTodoList.Dequeue();
			roughMap[currentRoomPos.x, currentRoomPos.y] = true;

			var allowedRooms = GetAllowedRooms(roughMap, currentRoomPos);

			int numAllowRooms = allowedRooms.Count;
			numAllowRooms = Mathf.Min(numAllowRooms, Settings.MaxRooms-numberRoomsMade);

			int minRooms = 0;
			if (roomTodoList.Count == 0)
			{
				minRooms = Mathf.Min(1, numAllowRooms);
			}

			int numberOfRoomsToAdd = Random.Range(minRooms, numAllowRooms);
			for (int loop = 0; loop < numberOfRoomsToAdd; loop++)
			{
				numberRoomsMade += 1;
				int index = Random.Range(0, allowedRooms.Count);
				roomTodoList.Enqueue(allowedRooms[index]);
				roughMap[allowedRooms[index].x, allowedRooms[index].y] = true;
				allowedRooms.RemoveAt(index);
			}
		}

		if (numberRoomsMade < Settings.MaxRooms)
		{
			for (int x = 0; x < Settings.MapSize; x++)
			{
				for (int y = 0; y < Settings.MapSize; y++)
				{
					if (!roughMap[x, y])
					{
						roughMap = MakeRoughMap(new Vector2Int(x, y), roughMap, numberRoomsMade);
					}
				}
			}
		}

		return roughMap;
	}

	string ArrayToString<T>(T[,] array)
	{
		var outputString = "";
		foreach (var item in array)
		{
			outputString += (item == null? "null" : item.ToString()) + ", ";
		}
		return outputString;
	}

	List<Vector2Int> GetAllowedRooms(bool[,] roughMap, Vector2Int currentRoomPos)
	{
		var allowedRooms = new List<Vector2Int>();
		if (currentRoomPos.x > 0 &&
			!roughMap[currentRoomPos.x-1, currentRoomPos.y])
		{
			allowedRooms.Add(new Vector2Int(currentRoomPos.x-1, currentRoomPos.y));
		}
		if (currentRoomPos.x < Settings.MapSize-1 &&
			!roughMap[currentRoomPos.x+1, currentRoomPos.y])
		{
			allowedRooms.Add(new Vector2Int(currentRoomPos.x+1, currentRoomPos.y));
		}
		
		if (currentRoomPos.y > 0&&
			!roughMap[currentRoomPos.x, currentRoomPos.y-1])
		{
			allowedRooms.Add(new Vector2Int(currentRoomPos.x, currentRoomPos.y-1));
		}
		if (currentRoomPos.y < Settings.MapSize-1 &&
			!roughMap[currentRoomPos.x, currentRoomPos.y+1])
		{
			allowedRooms.Add(new Vector2Int(currentRoomPos.x, currentRoomPos.y+1));
		}
		return allowedRooms;
	}
}
