using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartPickerUiController : MonoBehaviour
{
    [SerializeField] Body PlayerBody;
	[SerializeField] PickerBodyPartUi PickerBodyPart;

	[SerializeField] Button UseButton;
	[SerializeField] Button DiscardButton;
	[SerializeField] BodyPartSpriteLookup PartSpriteLookup;

	[SerializeField] DpadAnimator DpadAnimatorContoller;


	[Space]
    [Header("Debug")]
	[SerializeField] MonsterProfile profileData;
	[SerializeField] BodyPartData BodyPartData;
	

	void Start()
	{
		StartCoroutine(FlowController());
	}

	IEnumerator FlowController()
	{
		PlayerBody.SetProfileData(profileData);
		PlayerBody.ShowStats(true, Body.eBodyPartType.None, false, false, true);
		PickerBodyPart.SetupData(BodyPartData, PartSpriteLookup);
		

		UseButton.SetShow(true);
		DiscardButton.SetShow(true);
		UseButton.SetSelected(false);
		DiscardButton.SetSelected(false);
		Button currentButton = null;

		var dpadPos = new Vector2(0, UseButton.transform.position.y);
		DpadAnimatorContoller.JumpToPoint(dpadPos);
		DpadAnimatorContoller.SetShow(true);

		while (!SimpleInput.GetInputActive(EInput.A) || currentButton == null)
		{
			if (SimpleInput.GetInputActive(EInput.dpadLeft))
			{
				currentButton = UseButton;
				UseButton.SetSelected(true);
				DiscardButton.SetSelected(false);
			}
			else if (SimpleInput.GetInputActive(EInput.dpadRight))
			{
				currentButton = DiscardButton;
				UseButton.SetSelected(false);
				DiscardButton.SetSelected(true);
			}
			yield return null;
		}

		UseButton.SetShow(false);
		DiscardButton.SetShow(false);

		if (currentButton == DiscardButton)
		{
			Debug.Log($"Discard Body Part: {PickerBodyPart}");
			yield break;
		}
		else
		{
			Debug.Log($"wanting to use Body Part: {PickerBodyPart}");

			yield return StartCoroutine(SwapBodyPart(BodyPartData));
		}

		while (!SimpleInput.GetInputActive(EInput.A))
		{
			yield return null;
		}
	}

	IEnumerator SwapBodyPart(BodyPartData bodyPartData)
	{
		if (bodyPartData.BodyPartType == BodyPart.eBodyPartSlotType.Torso)
		{
			PlayerBody.TorsoPart.SetBodyPartData(PartSpriteLookup, bodyPartData, Body.eBodyPartType.Torso);
			yield break;
		}
		else if (bodyPartData.BodyPartType == BodyPart.eBodyPartSlotType.Leg)
		{
			PlayerBody.LegsPart.SetBodyPartData(PartSpriteLookup, bodyPartData, Body.eBodyPartType.Leg);
			yield break;
		}
		
		DpadAnimatorContoller.AnimateToPoint(PlayerBody.DPadGameTransform.position);

		BodyPart currentBodyPart = null;
		while (!SimpleInput.GetInputActive(EInput.A) || currentBodyPart == null)
		{
			PlayerBody.TorsoPart.ShowStats(true, false, true, true, true);

			PlayerBody.LeftArmPart.ShowStats(true, PlayerBody.LeftArmPart == currentBodyPart, false, true);
			PlayerBody.LeftArmPart.SetSprite(PartSpriteLookup, 
				PlayerBody.LeftArmPart == currentBodyPart? bodyPartData.MonsterType: EMonsterType.none);

			PlayerBody.RightArmPart.ShowStats(true, PlayerBody.RightArmPart == currentBodyPart, false, true);
			PlayerBody.RightArmPart.SetSprite(PartSpriteLookup, 
				PlayerBody.RightArmPart == currentBodyPart? bodyPartData.MonsterType: EMonsterType.none);

			PlayerBody.LegsPart.ShowStats(true, false, true, true, true);

			
			yield return null;

			var mostRecentDpad = SimpleInput.GetRecentDpad();

			if (!SimpleInput.GetInputActive(mostRecentDpad))
			{
				continue;
			}

			switch (mostRecentDpad)
			{
				case (EInput.dpadLeft):
				{
					currentBodyPart = PlayerBody.LeftArmPart;
					break;
				}
				case (EInput.dpadRight):
				{
					currentBodyPart = PlayerBody.RightArmPart;
					break;
				}
				default:
				{
					Debug.LogError($" unexpected button: {mostRecentDpad}");
					break;
				}
			}
		}

		currentBodyPart.SetBodyPartData(PartSpriteLookup, bodyPartData, currentBodyPart.BodyPartType);
	}
}
