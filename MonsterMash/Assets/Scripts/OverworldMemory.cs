using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class OverworldMemory : MonoBehaviour
{
	[SerializeField] MonsterGenerator PlayerMGen;

	public static PlayerProfile PlayerProfile;
	static Dictionary<int, MonsterProfile> EnemyProfiles = new Dictionary<int, MonsterProfile>();

	static Vector3 PlayerPos;
	static Dictionary<int, Vector3> EnemyPositions = new Dictionary<int, Vector3>();

	static int opponentID;
	static List<BodyPartData> BodyPartLootList = new List<BodyPartData>();

	void Awake()
	{
		if (PlayerProfile == null)
		{
			PlayerProfile = new PlayerProfile();
			PlayerProfile.CombatProfile = PlayerMGen.GetMonster(EMonsterType.Frankenstein);
		}
	}

	// Profiles
	public static void RecordEnemyProfile(MonsterProfile profile, int id)
	{
		// Set enemy profile
		if (!EnemyProfiles.ContainsKey(id))
		{
			EnemyProfiles.Add(id, profile);
		}
		else
		{
			EnemyProfiles[id] = profile;
		}
	}

	public static MonsterProfile GetEnemyProfile(int id)
	{
		return EnemyProfiles[id];
	}

	public static Dictionary<int, MonsterProfile> GetEnemyProfiles()
	{
		return EnemyProfiles;
	}

	// Get profiles for combat
	public static MonsterProfile GetCombatProfile(bool isPlayer)
	{
		if (isPlayer)
		{
			return PlayerProfile.CombatProfile;
		}
		else
		{
			return EnemyProfiles[opponentID];
		}
	}

	// Positions
	public static void RecordPosition(Vector3 pos, int id = -1)
	{
		// Save player pos
		if (id == -1)
		{
			PlayerPos = pos;
			return;
		}


		// Save enemy pos
		if (!EnemyPositions.ContainsKey(id))
		{
			EnemyPositions.Add(id, pos);
		}
		else
		{
			EnemyPositions[id] = pos;
		}
	}

	public static Vector3 GetEnemyPosition(int id)
	{
		return EnemyPositions[id];
	}

	public static Dictionary<int, Vector3> GetEnemyPositions()
	{
		return EnemyPositions;
	}

	// Update enemy id's after re-creation
	public static void UpdateID(int oldID, int newID)
	{
		// Change key position
		EnemyPositions.Add(newID, EnemyPositions[oldID]);
		EnemyPositions.Remove(oldID);

		// Change key profile
		EnemyProfiles.Add(newID, EnemyProfiles[oldID]);
		EnemyProfiles.Remove(oldID);
	}


	// Player position
	public static Vector3 GetPlayerPosition()
	{
		return PlayerPos;
	}

	// Opponent for combat
	public static int OpponentID
	{
		get { return opponentID; }
		set { opponentID = value; }
	}

	public static void OpponentBeaten(bool getLoot)
	{
		MMLogger.Log($"OpponentBeaten called with getLoot: {getLoot}");
		
		EnemyPositions.Remove(OpponentID);

		ClearLoot();
		if (getLoot)
		{
			var monPro = EnemyProfiles[opponentID];

			if(monPro.Torso != null &&
				(monPro.Torso.MonsterType == EMonsterType.lobster ||
				monPro.Torso.MonsterType == EMonsterType.shrimp))
			{
				BodyPartLootList.Add(monPro.Torso);
			}
			else
			{
				var toPickFrom = new List<BodyPartData>();
				if(monPro.LeftArm != null)
				{
					toPickFrom.Add(monPro.LeftArm);
				}
				if(monPro.RightArm != null)
				{
					toPickFrom.Add(monPro.RightArm);
				}
				if(monPro.Legs != null)
				{
					toPickFrom.Add(monPro.Legs);
				}

				int bodyPart = Random.Range(0, toPickFrom.Count);
				BodyPartLootList.Add(toPickFrom[bodyPart]);
			}
		}
		EnemyProfiles.Remove(OpponentID);
	}

	public static List<BodyPartData> GetLootProfile()
	{
		return BodyPartLootList;
	}

	public static void ClearLoot()
	{
		BodyPartLootList.Clear();
	}

	// To clear info // ~~~ change once spawning supplies id's instead of distruction
	public static void ClearEnemies()
	{
		EnemyPositions.Clear();
		EnemyProfiles.Clear();
		opponentID = 0;
	}

	public static void ClearAll()
	{
		PlayerProfile = null;
		EnemyProfiles.Clear();
		EnemyPositions.Clear();
		PlayerPos = Vector3.zero;
		opponentID = 0;
		BodyPartLootList.Clear();
	}
}
