using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterGenerator", menuName = "ScriptableObjects/MonsterGenerator")]
public class MonsterGenerator : ScriptableObject
{
    [System.Serializable]
    public class DefaultMonsterData
    {
        [System.Serializable]
        public struct TypeToGenerator
        {
            public BodyPart.eBodyPartSlotType Type;
            public BodyPartDataGenerator Generator;
        }
        public EMonsterType MonsterType;

        public List<TypeToGenerator> BodyPartGenerators = new List<TypeToGenerator>();
    }

    public List<DefaultMonsterData> Monsters = new List<DefaultMonsterData>();

    public MonsterProfile GetMonster(EMonsterType type)
    {
        MonsterProfile mp = new MonsterProfile();

        DefaultMonsterData data = Monsters.Find(d => d.MonsterType == type);

		mp.HeadType = type;
        mp.Torso = data.BodyPartGenerators.Find(g => g.Type == BodyPart.eBodyPartSlotType.Torso).Generator.GenerateBodyPartData();
        mp.LeftArm = data.BodyPartGenerators.Find(g => g.Type == BodyPart.eBodyPartSlotType.Arm).Generator.GenerateBodyPartData();
        mp.RightArm = data.BodyPartGenerators.Find(g => g.Type == BodyPart.eBodyPartSlotType.Arm).Generator.GenerateBodyPartData();
        mp.Legs = data.BodyPartGenerators.Find(g => g.Type == BodyPart.eBodyPartSlotType.Leg).Generator.GenerateBodyPartData();

        return mp;
    }


    // ~~~ generate specific body part
}
