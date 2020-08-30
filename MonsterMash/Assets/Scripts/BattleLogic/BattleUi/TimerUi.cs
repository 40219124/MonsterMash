using UnityEngine;
using UnityEngine.UI;
using System;


public class TimerUi : MonoBehaviour
{
	[SerializeField] Text TimerText;
	[SerializeField] Slider ActionTimeSlider;
	[SerializeField] Slider TimeUsedSlider;

	void Awake()
	{
		
	}

	void Update()
	{
		float maxTime = Settings.TurnTime;
		float timeLeft = BattleController.Instance.TurnTimeLeft;

		TimerText.text = $"{Math.Round(timeLeft, 2)}s";

		float timeUsed = maxTime-timeLeft;
		TimeUsedSlider.value = timeUsed/maxTime;

		float actionTime = timeUsed + BattleController.Instance.TimeLeftOfAction;
		ActionTimeSlider.value = actionTime/maxTime;
	}
}