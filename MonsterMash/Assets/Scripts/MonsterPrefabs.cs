using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterPrefabTable", menuName = "ScriptableObjects/MonsterPrefabTable")]
public class MonsterPrefabs : ScriptableObject
{
	[System.Serializable]
	public struct MonsterToPrefab
	{
		public EMonsterType Type;
		public Transform Prefab;
	}

	public List<MonsterToPrefab> Table = new List<MonsterToPrefab>();

}
