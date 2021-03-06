﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class ProceduralDungeon : MonoBehaviour
{
	public static ProceduralDungeon Instance;
	public static string RoomJsonFile = System.IO.Path.Combine("Assets", "Resources", "RoomData.txt");

	public int NumberOfDungeonsMade = 0;
	public MapRoom[,] DungeonMap = new MapRoom[Settings.MapSize, Settings.MapSize];
	public Room.eArea CurrentAreaType;
	public Vector2Int CurrentRoom;
	public Vector3 EnterPos;
	public Vector3 EnterMoveTargetPos;
	public AllRoomData AllRoomsData;
	bool DungeonGotBossRoom = false;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			StartCoroutine(Setup());
		}
	}

	IEnumerator Setup()
	{
		yield return StartCoroutine(LoadAllRoomData());
		GenerateMap(Room.eArea.Outdoors);
	}

	public MapRoom GetCurrentRoom()
	{
		return DungeonMap[CurrentRoom.x,CurrentRoom.y];
	}

	public void MoveRoom(Vector2Int roomDirection, Vector3 playerPos)
	{
		CurrentRoom += roomDirection;
		MMLogger.Log($"Moved To room {CurrentRoom} RoomID: {GetCurrentRoom().RoomData.RoomId}");

		var xPos = playerPos.x;
		if (playerPos.x == 0 ||
			playerPos.x == Room.GameWidth-1)
		{
			xPos = (Room.GameWidth-1)-xPos;
		}

		var yPos = playerPos.y;
		if (playerPos.y == 0 ||
			playerPos.y == Room.GameHeight-1)
		{
			yPos = (Room.GameHeight-1) - yPos;
		}

		EnterPos = new Vector3(xPos, yPos);
		EnterMoveTargetPos = EnterPos + (Vector3Int)roomDirection;
	}

	public bool IsDungeonCompleted()
	{
		foreach (var mapRoom in DungeonMap)
		{
			if (mapRoom != null && mapRoom.RoomState != ERoomState.Completed)
			{
				return false;
			}
		}
		return true;
	}

	public bool IsLastRoom(MapRoom current)
	{
		if (DungeonGotBossRoom)
		{
			return false;
		}
		foreach (MapRoom mapRoom in DungeonMap)
		{
			if(current == mapRoom)
			{
				continue;
			}
			else if (mapRoom != null && mapRoom.RoomState != ERoomState.Completed)
			{
				return false;
			}
		}
		return true;
	}

	public void MarkRoomAsBoss()
	{
		MMLogger.Log($"Trying to mark room {CurrentRoom} as boss room. DungeonGotBossRoom: {DungeonGotBossRoom}");
		if (!DungeonGotBossRoom)
		{
			DungeonMap[CurrentRoom.x, CurrentRoom.y].SetAsBossRoom();
			DungeonGotBossRoom = true;
			MMLogger.Log("WELL DONE YOU FOUND THE BOSS ROOM!");
		}
	}
	
	IEnumerator LoadAllRoomData()
	{
		string roomDataJson = "";
		var asset = Resources.Load<TextAsset>("RoomData");
		roomDataJson = asset.text;

		MMLogger.Log($"roomDataJson: {roomDataJson}");
		AllRoomsData = JsonUtility.FromJson<AllRoomData>(roomDataJson);
		MMLogger.Log($"AllRoomsData: {AllRoomsData}");
		yield return null;
	}

	public void GenerateMap(Room.eArea area)
	{
		DungeonGotBossRoom = false;
		CurrentAreaType = area;
		CurrentRoom = new Vector2Int(Random.Range(0, Settings.MapSize), Random.Range(0, Settings.MapSize));
		var roughMap = MakeRoughMap(CurrentRoom);
		MMLogger.Log($"Built roughMap: {ArrayToString(roughMap)}");

		BuildRealMap(roughMap, area);

		MMLogger.Log($"Built real map: {ArrayToString(DungeonMap)}");
		NumberOfDungeonsMade += 1;
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
						MMLogger.LogError($"AllRoomsData cannot find room {neededDoorPos} from area: {area}");
					}
				}
				DungeonMap[x,y] = mapRoom;
			}
		}

		DungeonMap[CurrentRoom.x,CurrentRoom.y].IsStartingRoom = true;
	}

	Room.eDoorPlaces GetRequiredDoorPostions(bool[,] roughMap, Vector2Int pos)
	{
		var neededDoors = Room.eDoorPlaces.None;

		if(pos.y+1 < Settings.MapSize && roughMap[pos.x, pos.y+1])
		{
			neededDoors |= Room.eDoorPlaces.Top;
		}

		if(pos.y-1 >= 0 && roughMap[pos.x, pos.y-1])
		{
			neededDoors |= Room.eDoorPlaces.Bottom;
		}

		if(pos.x+1 < Settings.MapSize && roughMap[pos.x+1, pos.y])
		{
			neededDoors |= Room.eDoorPlaces.Right;
		}

		if(pos.x-1 >= 0 && roughMap[pos.x-1, pos.y])
		{
			neededDoors |= Room.eDoorPlaces.Left;
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
