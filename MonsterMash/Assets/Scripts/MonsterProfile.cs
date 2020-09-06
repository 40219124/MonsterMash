using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EMonsterType { none = -1, Frankenstein, skeleton, mantis, lobster, shrimp };

[System.Serializable]
public class MonsterProfile
{
    public BodyPartData Torso;
    public BodyPartData LeftArm;
    public BodyPartData RightArm;
    public BodyPartData Legs;

	public void HealToMax(bool restoreDead)
	{
		Debug.Log("Healing player to max");
		Torso.RestoreHealth(restoreDead);
		LeftArm.RestoreHealth(restoreDead);
		RightArm.RestoreHealth(restoreDead);
		Legs.RestoreHealth(restoreDead);
	}
}
