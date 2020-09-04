using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickerBodyPartUi : MonoBehaviour
{
	public BodyPartData PartData { get; private set; }
	[SerializeField] SpriteRenderer BodyPartImage;
	[SerializeField] Animator PartAnimator;
	[SerializeField] BodyPartUi StatBox;

	public void SetupData(BodyPartData partData, BodyPartSpriteLookup partSpriteLookup)
	{
		if(partData == null)
		{
			gameObject.SetActive(false);
		}
		else
		{
			PartData = partData;
			BodyPartImage.sprite = partSpriteLookup.GetBodyPartSprite(PartData.BodyPartType, PartData.MonsterType);


			StatBox.Show(true, false, PartData.BodyPartType != BodyPart.eBodyPartSlotType.Torso, false);
			StatBox.SetDamageNumber(PartData.BodyPartType != BodyPart.eBodyPartSlotType.Torso, PartData.Damage);
			StatBox.SetTimeNumber(PartData.BodyPartType != BodyPart.eBodyPartSlotType.Torso, PartData.AttackTimer);

			StatBox.SetHealthNumber(true, PartData.HealthCurrent);
			StatBox.SetArmourNumber(true, PartData.Armour);
		}
	}

	public override string ToString()
	{
		return $"PickerBodyPartUi({PartData})";
	} 
}
