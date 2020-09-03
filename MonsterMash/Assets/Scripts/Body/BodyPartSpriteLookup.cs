using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBodyPart", menuName = "ScriptableObjects/BodyPartSpriteLookup")]
public class BodyPartSpriteLookup : ScriptableObject
{
	[System.Serializable]
	public class BodyPartSpriteData
	{
		public Body.eBodyPartType BodyPartType;
		public EMonsterType MonsterType;
		public Sprite BodyPartSprite;
	}

	public List<BodyPartSpriteData> BodyPartSpriteLookupList;

	public Sprite GetBodyPartSprite(BodyPart.eBodyPartSlotType bodyPartSlotType, EMonsterType monsterType)
	{
		Body.eBodyPartType bodyPartType = Body.eBodyPartType.None;
		switch (bodyPartSlotType)
		{
			case (BodyPart.eBodyPartSlotType.Torso):
			{
				bodyPartType = Body.eBodyPartType.Torso;
				break;
			}
			case (BodyPart.eBodyPartSlotType.Arm):
			{
				bodyPartType = Body.eBodyPartType.LeftArm;
				break;
			}
			case (BodyPart.eBodyPartSlotType.Leg):
			{
				bodyPartType = Body.eBodyPartType.Leg;
				break;
			}
			default:
			{
				Debug.LogError($"Unexpected enum type: {bodyPartSlotType}");
				break;
			}
		}
		return GetBodyPartSprite(bodyPartType, monsterType);
	}
	public Sprite GetBodyPartSprite(Body.eBodyPartType bodyPartType, EMonsterType monsterType)
	{
		foreach (var bodyPartSpriteLookup in BodyPartSpriteLookupList)
		{
			if(bodyPartSpriteLookup.BodyPartType == bodyPartType &&
				bodyPartSpriteLookup.MonsterType == monsterType)
			{
				return bodyPartSpriteLookup.BodyPartSprite;
			}
		}

		Debug.LogWarning($"Could not find sprite with bodyPartType: {bodyPartType}, monsterType: {monsterType}");
		return null;
	}
}
