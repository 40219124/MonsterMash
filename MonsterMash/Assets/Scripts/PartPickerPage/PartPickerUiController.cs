using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartPickerUiController : MonoBehaviour
{
	[SerializeField] AudioClip MoveAudioClip;
	[SerializeField] AudioClip SelectedAudioClip;

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
			MMLogger.LogError($"not enough PickerBodyParts setup (num setup: {PickerBodyParts.Count}) num neeed: {bodyPartsDataList.Count}");
		}
		

		foreach (var pickerBodyPart in PickerBodyParts)
		{
			if (pickerBodyPart.gameObject.activeSelf)
			{
				yield return StartCoroutine(PickPartUse(pickerBodyPart));
			}
		}
		MMLogger.Log("Finished picking use of parts");

		DpadAnimatorContoller.SetShow(false);
		UseButton.SetSelected(false);
		DiscardButton.SetSelected(false);
		PlayerBody.ShowStats(true, Body.eBodyPartType.None, false, false, true);

		UseButton.SetShow(false);
		DiscardButton.SetShow(false);
		PressButtonPrompt.SetActive(true);

		while  (SimpleInput.GetInputState(EInput.A) != EButtonState.Released &&
				SimpleInput.GetInputState(EInput.B) != EButtonState.Released &&
				SimpleInput.GetInputState(EInput.Select) != EButtonState.Released)
		{
			yield return null;
		}
		FindObjectOfType<PartPickerManager>().LeavePicker();
	}

	IEnumerator PickPartUse(PickerBodyPartUi pickerBodyPart)
	{
		MMLogger.Log($"picking use of part: {pickerBodyPart}");
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

		while (SimpleInput.GetInputState(EInput.A) != EButtonState.Released || currentButton == null)
		{
			if (SimpleInput.GetInputActive(EInput.dpadLeft) && currentButton != UseButton)
			{
				currentButton = UseButton;
				UseButton.SetSelected(true);
				DiscardButton.SetSelected(false);
				AudioSource.PlayClipAtPoint(MoveAudioClip, Vector3.zero);
			}
			else if (SimpleInput.GetInputActive(EInput.dpadRight) && currentButton != DiscardButton)
			{
				currentButton = DiscardButton;
				UseButton.SetSelected(false);
				DiscardButton.SetSelected(true);
				AudioSource.PlayClipAtPoint(MoveAudioClip, Vector3.zero);
			}
			yield return null;
		}
		AudioSource.PlayClipAtPoint(SelectedAudioClip, Vector3.zero);

		UseButton.SetShow(false);
		DiscardButton.SetShow(false);

		if (currentButton == DiscardButton)
		{
			MMLogger.Log($"Discard Body Part: {pickerBodyPart}");
		}
		else
		{
			MMLogger.Log($"wanting to use Body Part: {pickerBodyPart}");

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
			while (SimpleInput.GetInputState(EInput.A) != EButtonState.Released || currentBodyPart == null)
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
						if (currentBodyPart != PlayerBody.LeftArmPart)
						{
							AudioSource.PlayClipAtPoint(MoveAudioClip, Vector3.zero);
						}
						currentBodyPart = PlayerBody.LeftArmPart;
						break;
					}
					case (EInput.dpadRight):
					{
						if (currentBodyPart != PlayerBody.RightArmPart)
						{
							AudioSource.PlayClipAtPoint(MoveAudioClip, Vector3.zero);
						}
						currentBodyPart = PlayerBody.RightArmPart;
						break;
					}
				}
			}
			AudioSource.PlayClipAtPoint(SelectedAudioClip, Vector3.zero);
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
				MMLogger.LogError($"unexpected eBodyPartType: {bodyPartToOverWrite.BodyPartType}");
				break;
			}
		}
		bodyPartToOverWrite.SetBodyPartData(PartSpriteLookup, dataToWrite, bodyPartToOverWrite.BodyPartType);
	}
}
