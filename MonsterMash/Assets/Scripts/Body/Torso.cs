using UnityEngine;
using UnityEngine.UI;
using System;

public class Torso: BodyPart
{
	[SerializeField] SpriteRenderer HeadPartImage;
	public override void ShowStats(bool show, bool selected, bool isOurTurn, bool forceComplex, bool forceDisable=false)
	{
		bool shouldShow = show && IsAlive;
		StatBox.Show(shouldShow, selected, showComplex:false, isOurTurn);

		StatBox.SetHealthNumber(show, CurrentHealth);
		StatBox.SetArmourNumber(show, Armour);
	}

	public void SetBodyPartData(BodyPartSpriteLookup bodyPartImageLookup, BodyPartData data, Body.eBodyPartType bodyPartType, EMonsterType headType)
    {
		base.SetBodyPartData(bodyPartImageLookup, data, bodyPartType);
		HeadPartImage.sprite = bodyPartImageLookup.GetBodyPartSprite(Body.eBodyPartType.None, headType);
    }
}