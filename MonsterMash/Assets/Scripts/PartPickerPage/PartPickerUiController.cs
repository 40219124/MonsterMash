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

			DpadAnimatorContoller.AnimateBetweenPoints(dpadPos, PlayerBody.DPadGameTransform.position);

			BodyPart currentBodyPart = null;
			while (!SimpleInput.GetInputActive(EInput.A) || currentBodyPart == null)
			{
				yield return null;
			}
			currentBodyPart.SetBodyPartData(PartSpriteLookup, BodyPartData, currentBodyPart.BodyPartType);
		}

		while (!SimpleInput.GetInputActive(EInput.A))
		{
			yield return null;
		}
	}
}
