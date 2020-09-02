using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBodyPart", menuName = "ScriptableObjects/BodyPartData")]
public class BodyPartDataGenerator : ScriptableObject
{
    public BodyPart.eBodyPartSlotType BodyPartType;
    public EMonsterType MonsterType;

    public int HealthAvg;
    public int HealthVariation;
    public int ArmourAvg;
    public int ArmourVariation;
    public int DamageAvg;
    public int DamageVariation;
    public int AttackTime;

    public BodyPartData GenerateBodyPartData()
    {
        return new BodyPartData(
            BodyPartType,
            MonsterType,
            HealthAvg + Random.Range(-HealthVariation, HealthVariation + 1),
            ArmourAvg + Random.Range(-ArmourVariation, ArmourVariation + 1),
            DamageAvg + Random.Range(-DamageVariation, DamageVariation + 1),
            AttackTime);
    }
}
