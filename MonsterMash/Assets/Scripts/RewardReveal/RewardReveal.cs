using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardReveal : MonoBehaviour
{
	[SerializeField] Animator ScreenAnimator;

	
    void Awake()
    {
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
