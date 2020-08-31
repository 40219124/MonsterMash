using UnityEngine;
using UnityEngine.UI;
using System;


public class TimerUi : MonoBehaviour
{
	[SerializeField] Transform ActionTimeSlider;
	[SerializeField] Transform TimeUsedSlider;

	[SerializeField] Transform BarPerant;

	void Awake()
	{
		
	}

	void Update()
	{
		float maxTime = Settings.TurnTime;
		float timeLeft = BattleController.Instance.TurnTimeLeft;

		float timeUsed = maxTime-timeLeft;

		float actionTime = timeUsed + BattleController.Instance.TimeLeftOfAction;

		if (TimeUsedSlider != null)
		{
			TimeUsedSlider.localScale = new Vector3(timeUsed/maxTime, 1, 1);
		}

		if (ActionTimeSlider != null)
		{
			ActionTimeSlider.localScale = new Vector3(actionTime/maxTime, 1, 1);
		}
	}
}