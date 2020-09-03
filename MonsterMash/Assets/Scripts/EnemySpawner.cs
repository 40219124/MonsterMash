using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public struct MonsterPosition
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
        foreach (MonsterPosition enemy in SpawnLocations)
        {
            Transform e = Instantiate(MPTable.Table.Find(p => p.Type == enemy.type).Prefab, this.transform);
            e.position = enemy.pos;
            e.GetComponent<OverworldAgent>().Profile = MGen.GetMonster(EMonsterType.skeleton);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
