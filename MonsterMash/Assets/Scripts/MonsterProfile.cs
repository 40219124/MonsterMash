using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EMonsterType { none = -1, Frankenstein, skeleton, mantis, lobster, shrimp, slug};

[System.Serializable]
public class MonsterProfile
{
	public EMonsterType HeadType;
    public BodyPartData Torso;
    public BodyPartData LeftArm;
    public BodyPartData RightArm;
    public BodyPartData Legs;

	public void HealToMax(bool restoreDead)
	{
		MMLogger.Log("Healing player to max");
		Torso.RestoreHealth(restoreDead);
		LeftArm.RestoreHealth(restoreDead);
		RightArm.RestoreHealth(restoreDead);
		Legs.RestoreHealth(restoreDead);
	}
}
