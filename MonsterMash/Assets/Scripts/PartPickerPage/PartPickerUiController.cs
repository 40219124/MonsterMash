using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartPickerUiController : MonoBehaviour
{
    [SerializeField] Body PlayerBody;
	[SerializeField] List<PickerBodyPartUi> PickerBodyParts;

	[SerializeField] Button UseButton;
	[SerializeField] Button DiscardButton;
	[SerializeField] BodyPartSpriteLookup PartSpriteLookup;

	[SerializeField] DpadAnimator DpadAnimatorContoller;

	[SerializeField] GameObject PressButtonPrompt;


	MonsterProfile ProfileData;

	public void Setup(MonsterProfile profileData, List<BodyPartData> bodyPartsDataList)
	{
		ProfileData = profileData;
		PlayerBody.SetProfileData(profileData);
		StartCoroutine(FlowController(bodyPartsDataList));
	}

	IEnumerator FlowController(List<BodyPartData> bodyPartsDataList)
	{
		PressButtonPrompt.SetActive(false);
		PlayerBody.ShowStats(true, Body.eBodyPartType.None, false, false, true);

		int loop = 0;
		foreach (var pickerBodyPart in PickerBodyParts)
		{

			pickerBodyPart.SetupData((bodyPartsDataList != null && loop < bodyPartsDataList.Count)? bodyPartsDataList[loop] : null , PartSpriteLookup);
			loop += 1;
		}

		if (bodyPartsDataList != null && loop < bodyPartsDataList.Count)
		{
			Debug.LogError($"not enough PickerBodyParts setup (num setup: {PickerBodyParts.Count}) num neeed: {bodyPartsDataList.Count}");
		}
		

		foreach (var pickerBodyPart in PickerBodyParts)
		{
			if (pickerBodyPart.gameObject.activeSelf)
			{
				yield return StartCoroutine(PickPartUse(pickerBodyPart));
			}
		}
		Debug.Log("Finished picking use of parts");

		DpadAnimatorContoller.SetShow(false);
		UseButton.SetSelected(false);
		DiscardButton.SetSelected(false);
		PlayerBody.ShowStats(true, Body.eBodyPartType.None, false, false, true);

		UseButton.SetShow(false);
		DiscardButton.SetShow(false);
		PressButtonPrompt.SetActive(true);

		while  (SimpleInput.GetInputState(EInput.A) != EButtonState.Pressed &&
				SimpleInput.GetInputState(EInput.B) != EButtonState.Pressed &&
				SimpleInput.GetInputState(EInput.Select) != EButtonState.Pressed)
		{
			yield return null;
		}
		FindObjectOfType<PartPickerManager>().LeavePicker();
	}

	IEnumerator PickPartUse(PickerBodyPartUi pickerBodyPart)
	{
		Debug.Log($"picking use of part: {pickerBodyPart}");
		UseButton.SetShow(true);
		DiscardButton.SetShow(true);
		UseButton.SetButtonSize(true);
		DiscardButton.SetButtonSize(true);
		pickerBodyPart.SetSelected(true);

		UseButton.SetSelected(false);
		DiscardButton.SetSelected(false);
		Button currentButton = null;

		var dpadPos = new Vector2(0, UseButton.transform.position.y);
		DpadAnimatorContoller.JumpToPoint(dpadPos);
		DpadAnimatorContoller.SetShow(true);

		while (SimpleInput.GetInputState(EInput.A) != EButtonState.Pressed || currentButton == null)
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
			Debug.Log($"Discard Body Part: {pickerBodyPart}");
		}
		else
		{
			Debug.Log($"wanting to use Body Part: {pickerBodyPart}");

			yield return StartCoroutine(SwapBodyPart(pickerBodyPart));
		}
		pickerBodyPart.gameObject.SetActive(false);
	}

	IEnumerator SwapBodyPart(PickerBodyPartUi pickerBodyPart)
	{
		var bodyPartData = pickerBodyPart.PartData;

		if (bodyPartData.BodyPartType == BodyPart.eBodyPartSlotType.Torso)
		{
			SwapPart(PlayerBody.TorsoPart, bodyPartData);
			yield break;
		}
		else if (bodyPartData.BodyPartType == BodyPart.eBodyPartSlotType.Leg)
		{
			SwapPart(PlayerBody.LegsPart, bodyPartData);
			yield break;
		}
		else if (!PlayerBody.LeftArmPart.IsAlive)
		{
			SwapPart(PlayerBody.LeftArmPart, bodyPartData);
			yield break;
		}
		else if (!PlayerBody.RightArmPart.IsAlive)
		{
			SwapPart(PlayerBody.RightArmPart, bodyPartData);
			yield break;
		}
		else
		{
			DpadAnimatorContoller.AnimateToPoint(PlayerBody.DPadGameTransform.position);

			BodyPart currentBodyPart = null;
			while (SimpleInput.GetInputState(EInput.A) != EButtonState.Pressed || currentBodyPart == null)
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
				}
			}
			SwapPart(currentBodyPart, bodyPartData);
		}
		pickerBodyPart.gameObject.SetActive(false);
	}

	void SwapPart(BodyPart bodyPartToOverWrite, BodyPartData dataToWrite)
	{
		switch (bodyPartToOverWrite.BodyPartType)
		{
			case (Body.eBodyPartType.Torso):
			{
				ProfileData.Torso = dataToWrite;
				break;
			}
			case (Body.eBodyPartType.LeftArm):
			{
				ProfileData.LeftArm = dataToWrite;
				break;
			}
			case (Body.eBodyPartType.RightArm):
			{
				ProfileData.RightArm = dataToWrite;
				break;
			}
			case (Body.eBodyPartType.Leg):
			{
				ProfileData.Legs = dataToWrite;
				break;
			}
			default:
			{
				Debug.LogError($"unexpected eBodyPartType: {bodyPartToOverWrite.BodyPartType}");
				break;
			}
		}
		bodyPartToOverWrite.SetBodyPartData(PartSpriteLookup, dataToWrite, bodyPartToOverWrite.BodyPartType);
	}
}
