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
		var battleController = BattleController.Instance;

		float maxTime = Settings.TurnTime;
		float timeLeft = battleController.TurnTimeLeft;

		float timeUsed = maxTime-timeLeft;

		float actionTime = timeUsed + battleController.TimeLeftOfAction;

		BarPerant.localScale = new Vector3(battleController.BattleState == BattleController.eBattleState.PlayerTurn? 1 : -1, 1, 1);

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