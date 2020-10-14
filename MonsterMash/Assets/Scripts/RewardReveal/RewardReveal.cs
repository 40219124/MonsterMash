using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardReveal : MonoBehaviour
{
	[SerializeField] Animator ScreenAnimator;
	[SerializeField] PickerBodyPartUi PickerBodyPart;
	[SerializeField] BodyPartSpriteLookup SpriteLookup;

	
    void Awake()
    {
		var parts = OverworldMemory.GetLootProfile();
		if (parts != null && parts.Count > 0)
		{
			PickerBodyPart.SetupData(parts[0], SpriteLookup);
			pickerBodyPart.SetSelected(true);
		}
		
        ScreenAnimator.SetBool("ShowReward", true);
    }

    void Update()
    {
		if (SimpleInput.GetInputState(EInput.A) == EButtonState.Released)
		{
        	ScreenAnimator.SetBool("ShowReward", false);
		}
    }

	public void RevealFinishedClosedEvent()
	{
		FlowManager.Instance.TransToPicker(Settings.RewardReveal);
	}
}
