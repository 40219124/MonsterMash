using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BodyPartData
{
    public BodyPart.eBodyPartSlotType BodyPartType;
    public EMonsterType MonsterType;

    public int HealthMaximum;
    public int HealthCurrent;
    public int Armour;
    public int Damage;
    public int AttackTimer;

    public override string ToString()
    {
        return $"BodyPartType: {BodyPartType}, MonsterType: {MonsterType}, health:{HealthCurrent}";
    }

    public BodyPartData(BodyPart.eBodyPartSlotType _bodyPartType, EMonsterType _monsterType, int _health, int _armour, int _damage, int _attackTimer)
    {
        //Body part type
        if (_bodyPartType == BodyPart.eBodyPartSlotType.None)
        {
            MMLogger.LogError("Body part type not given.");
            return;
        }
        BodyPartType = _bodyPartType;

        //MonsterType
        if (_monsterType == EMonsterType.none)
        {
            MMLogger.LogError("Monster type not given.");
            return;
        }
        MonsterType = _monsterType;

        // Health
        if (_health < 1)
        {
            MMLogger.LogWarning($"Health data supplied too low: {_health}");
        }
        HealthMaximum = Mathf.Max(1, _health);
        HealthCurrent = HealthMaximum;

        // Armour
        if (_armour < 0)
        {
            MMLogger.LogWarning($"Armour data supplied too low: {_armour}");
        }
        Armour = Mathf.Max(0, _armour);

        // Damage
        if (_damage < 0)
        {
            MMLogger.LogWarning($"Damage data supplied too low: {_damage}");
        }
        Damage = Mathf.Max(0, _damage);

        // AttackTimer
        if (_attackTimer < 1)
        {
            MMLogger.LogWarning($"Attack timer supplied too low: {_attackTimer}");
        }
        else if (_attackTimer > Settings.TurnTime)
        {
            MMLogger.LogWarning($"Attack timer supplied too high: {_attackTimer}");
        }
        AttackTimer = Mathf.Clamp(_attackTimer, 1, (int)Settings.TurnTime - 1);
    }

    public BodyPartData(BodyPartData data)
    {
        BodyPartType = data.BodyPartType;
        MonsterType = data.MonsterType;

        HealthMaximum = data.HealthMaximum;
        HealthCurrent = data.HealthCurrent;
        Armour = data.Armour;
        Damage = data.Damage;
        AttackTimer = data.AttackTimer;
    }

    public void RestoreHealth(bool restoreDead)
    {
		if(restoreDead || HealthCurrent > 0)
		{
			MMLogger.Log($"restore health on {this}");
			HealthCurrent = HealthMaximum;
		}
    }

	public void Upgrade(bool restoreDead)
	{
		if (restoreDead || HealthCurrent > 0)
		{
			HealthMaximum = (int) (HealthMaximum * Settings.UpgradeBodyPartHealthMultiplier);
			Damage = (int) (Damage * Settings.UpgradeBodyPartDamageMultiplier);
			Armour = (int) (Armour * Settings.UpgradeBodyPartArmorMultiplier);
			RestoreHealth(restoreDead);
		}
	}
}
