using UnityEngine;
using UnityEngine.UI;
using System;


public class TimerUi : MonoBehaviour
{
	[SerializeField] NumberRender TimeLeftNumber;

	[SerializeField] Transform ActionTimeSlider;
	[SerializeField] Transform TimeUsedSlider;
	[SerializeField] Transform BubbleVfx;

	[SerializeField] Transform BarPerant;

	[SerializeField] float StartPos;
	[SerializeField] float EndPos;

	void Update()
	{
		var battleController = BattleController.Instance;

		float maxTime = Settings.TurnTime;
		float timeLeft = battleController.TurnTimeLeft;

		TimeLeftNumber.SetNumber((int)Math.Floor(timeLeft));

		float timeUsed = maxTime-timeLeft;

		float actionTime = timeUsed + battleController.TimeLeftOfAction;

		float vfxProgress = timeUsed/maxTime;
		if (battleController.BattleState == BattleController.eBattleState.PlayerTurn)
		{
			BarPerant.localScale = new Vector3(1, 1, 1);

			TimeUsedSlider.position = new Vector3(StartPos, TimeUsedSlider.position.y, 0);
			ActionTimeSlider.position = new Vector3(StartPos, ActionTimeSlider.position.y, 0);
		}
		else
		{
			BarPerant.localScale = new Vector3(-1, 1, 1);
			TimeUsedSlider.position = new Vector3(EndPos, TimeUsedSlider.position.y, 0);
			ActionTimeSlider.position = new Vector3(EndPos, ActionTimeSlider.position.y, 0);
			vfxProgress = 1-vfxProgress;
		}

		TimeUsedSlider.localScale = new Vector3(timeUsed/maxTime, 1, 1);
		ActionTimeSlider.localScale = new Vector3(actionTime/maxTime, 1, 1);


		if (BubbleVfx != null)
		{
			vfxProgress = StartPos + vfxProgress*(EndPos-StartPos);
			BubbleVfx.position = new Vector3(vfxProgress, BubbleVfx.position.y, BubbleVfx.position.z);
		}
	}
}