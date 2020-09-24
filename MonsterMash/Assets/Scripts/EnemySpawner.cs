using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	[System.Serializable]
	public class MonsterPosition
	{
		public EMonsterType type;
		public Vector3 pos;
	}

	public MonsterPrefabs MPTable;
	public MonsterGenerator MGen;

	public List<MonsterPosition> SpawnLocations = new List<MonsterPosition>();
	// Start is called before the first frame update
	void Start()
	{
	}

	public void SpawnEnemies(bool fromMemory = false)
	{
		if (fromMemory)
		{
			List<MonsterPosition> locations = new List<MonsterPosition>();
			List<(int, int)> oldToNew = new List<(int, int)>();
			foreach (var idPos in OverworldMemory.GetEnemyPositions())
			{
				MonsterProfile monPro = OverworldMemory.GetEnemyProfile(idPos.Key);
				EMonsterType type = monPro.Torso.MonsterType;

				Transform e = Instantiate(MPTable.Table.Find(p => p.Type == type).Prefab, this.transform);
				e.position = idPos.Value;
				e.GetComponent<OverworldAgent>().Profile = monPro;

				// update memory with new trans id's
				oldToNew.Add((idPos.Key, e.GetInstanceID()));
			}

			foreach (var pair in oldToNew)
			{
				OverworldMemory.UpdateID(pair.Item1, pair.Item2);
			}

		}
		else
		{
			foreach (MonsterPosition enemy in SpawnLocations)
			{
				MonsterProfile monPro = MGen.GetMonster(enemy.type);

				Transform e = Instantiate(MPTable.Table.Find(p => p.Type == enemy.type).Prefab, this.transform);
				e.position = enemy.pos;
				e.GetComponent<OverworldAgent>().Profile = monPro;

				// update memory with new trans id's
				OverworldMemory.RecordProfile(monPro, e.GetInstanceID());
			}
		}

	}
}
