using UnityEngine;
using UnityEngine.UI;
using System;


public class TimerUi : MonoBehaviour
{

	[SerializeField] AudioClip TurnStartAudioClip;
	[SerializeField] AudioClip TurnEndAudioClip;

	[SerializeField] Animator TimerAnimator;
	[SerializeField] NumberRender TimeLeftNumber;

	[SerializeField] Transform ActionTimeSlider;
	[SerializeField] Transform TimeUsedSlider;
	[SerializeField] Transform BubbleVfx;

	[SerializeField] Transform BarPerant;

	[SerializeField] float StartPos;
	[SerializeField] float EndPos;

	float LastVfxProgress = -1;
	BattleController.eBattleState LastBattleState;

	void Awake()
	{
		TimeLeftNumber.UseLargeNumbers = true;
	}

	void LateUpdate()
	{
		var battleController = BattleController.Instance;

		bool gameOver = battleController.BattleState == BattleController.eBattleState.EnemyWon ||
						battleController.BattleState == BattleController.eBattleState.PlayerWon;

		TimerAnimator.SetBool("GamePlaying", battleController.BattleState != BattleController.eBattleState.BattleIntro && !gameOver);

		//TimerAnimator.SetBool("BarMoving", vfxProgress != LastVfxProgress);
		TimerAnimator.SetBool("BarMoving", false);

		if (gameOver)
		{
			return;
		}

		float maxTime = Settings.TurnTime;
		float timeLeft = battleController.TurnTimeLeft;
		float timeUsed = maxTime-timeLeft;
		float actionTime = timeUsed + battleController.TimeLeftOfAction;

		if (battleController.BattleState == BattleController.eBattleState.BattleIntro &&
			Settings.BattleIntroMaxTime >= 0)
		{
			maxTime = Settings.BattleIntroMaxTime;
			actionTime = 0;
			timeLeft = Settings.BattleIntroMaxTime - battleController.TimeSinceActionStarted;
			timeUsed = battleController.TimeSinceActionStarted;
		}
		else if (battleController.BattleState == BattleController.eBattleState.TurnTransition)
		{
			maxTime = Settings.TurnTransitionTime;
			timeLeft = battleController.TurnTransitionTimeLeft;
			actionTime = 0;
			timeUsed = timeLeft;
		}

		TimeLeftNumber.SetNumber((int)Math.Floor(timeLeft));

		float vfxProgress = timeUsed/maxTime;
		if (battleController.CurrentAgent == battleController.Player)
		{
			BarPerant.localScale = new Vector3(1, 1, 1);

			TimeUsedSlider.position = new Vector3(StartPos, TimeUsedSlider.position.y, 0);
			ActionTimeSlider.position = new Vector3(StartPos, ActionTimeSlider.position.y, 0);
		}
		else if (battleController.CurrentAgent == battleController.Enemy)
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

		if (LastBattleState == BattleController.eBattleState.TurnTransition && 
			(battleController.BattleState == BattleController.eBattleState.PlayerTurn ||
			battleController.BattleState == BattleController.eBattleState.EnemyTurn))
		{
			AudioSource.PlayClipAtPoint(TurnStartAudioClip, transform.position);
		}
		else if ((LastBattleState == BattleController.eBattleState.PlayerTurn ||
				LastBattleState == BattleController.eBattleState.EnemyTurn) && 
				battleController.BattleState == BattleController.eBattleState.TurnTransition)
		{
			AudioSource.PlayClipAtPoint(TurnEndAudioClip, transform.position);
		}

		LastBattleState = battleController.BattleState;
		LastVfxProgress = vfxProgress;
	}
}