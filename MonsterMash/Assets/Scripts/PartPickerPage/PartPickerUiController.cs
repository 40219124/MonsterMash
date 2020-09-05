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


	[Space]
    [Header("Debug")]
	[SerializeField] MonsterProfile ProfileData;
	[SerializeField] List<BodyPartData> BodyPartsData;
	

	void Start()
	{
		Setup(ProfileData, BodyPartsData);
	}

	public void Setup(MonsterProfile profileData, List<BodyPartData> bodyPartsDataList)
	{
		PlayerBody.SetProfileData(profileData);
		StartCoroutine(FlowController(bodyPartsDataList));
	}

	IEnumerator FlowController(List<BodyPartData> bodyPartsDataList)
	{
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
			PlayerBody.TorsoPart.SetBodyPartData(PartSpriteLookup, bodyPartData, Body.eBodyPartType.Torso);
			yield break;
		}
		else if (bodyPartData.BodyPartType == BodyPart.eBodyPartSlotType.Leg)
		{
			PlayerBody.LegsPart.SetBodyPartData(PartSpriteLookup, bodyPartData, Body.eBodyPartType.Leg);
			yield break;
		}
		else if (!PlayerBody.LeftArmPart.IsAlive)
		{
			PlayerBody.LeftArmPart.SetBodyPartData(PartSpriteLookup, bodyPartData, Body.eBodyPartType.LeftArm);
			yield break;
		}
		else if (!PlayerBody.RightArmPart.IsAlive)
		{
			PlayerBody.RightArmPart.SetBodyPartData(PartSpriteLookup, bodyPartData, Body.eBodyPartType.RightArm);
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
			currentBodyPart.SetBodyPartData(PartSpriteLookup, bodyPartData, currentBodyPart.BodyPartType);
		}
		pickerBodyPart.gameObject.SetActive(false);
	}
}
