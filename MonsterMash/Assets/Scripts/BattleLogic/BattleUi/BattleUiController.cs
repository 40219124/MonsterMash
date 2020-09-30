using UnityEngine;
using UnityEngine.UI;
using System;

public class BattleUiController: MonoBehaviour
{
	[SerializeField] AudioClip SelectedAudioClip;

	[SerializeField] GameObject OutSideBackground;
	[SerializeField] GameObject InSideBackground;

	[SerializeField] DpadAnimator DpadAnimatorContoller;
	[SerializeField] GameObject ButtonAPrompt;
	[SerializeField] TextMesh GameStateText;

	[SerializeField] Transform TurnArrow;

	bool ForceShowComplexStats;

	Body.eBodyPartType LastSelectedAttacker;
	Body.eBodyPartType LastSelectedTarget;

	void Awake()
	{
		var roomType = Room.eArea.Indoors;
		if(ProceduralDungeon.Instance != null)
		{
			roomType = ProceduralDungeon.Instance.CurrentAreaType;
		}
		OutSideBackground.SetActive(roomType == Room.eArea.Outdoors);
		InSideBackground.SetActive(roomType == Room.eArea.Indoors);
	}

	void LateUpdate()
	{
		var battleController = BattleController.Instance;
		
		bool isGameOver = battleController.BattleState == BattleController.eBattleState.PlayerWon ||
						  battleController.BattleState == BattleController.eBattleState.EnemyWon;

		ButtonAPrompt.SetActive(battleController.BattleState == BattleController.eBattleState.BattleIntro || isGameOver);

		var currentAgent = battleController.CurrentAgent;

		TurnArrow.gameObject.SetActive(!isGameOver);
		TurnArrow.position = currentAgent.Body.Root.position;

		if (battleController.BattleState == BattleController.eBattleState.BattleIntro)
		{
			DpadAnimatorContoller.SetShow(false);
			battleController.Player.Body.ShowStats(true, Body.eBodyPartType.None, false, isOurTurn:false, true);
			battleController.Enemy.Body.ShowStats(true, Body.eBodyPartType.None, false, isOurTurn:false, true);
			GameStateText.text = "READY?";
			return;
		}
		else if (battleController.BattleState == BattleController.eBattleState.PlayerWon)
		{
			GameStateText.text = "YOU WON";
		}
		else if (battleController.BattleState == BattleController.eBattleState.EnemyWon)
		{
			GameStateText.text = "YOU LOST";
		}

		var opponent = currentAgent.Opponent;

		bool isPlayer = currentAgent.ControlType == Agent.eControlType.Player;

		if (SimpleInput.GetInputState(EInput.Select) == EButtonState.Pressed)
		{
			ForceShowComplexStats = !ForceShowComplexStats;
		}

		bool hasVaildAction = battleController.HasValidAttcker(currentAgent);


		float preShowTime = Settings.PreShowBattleUiTime;
		if (battleController.CurrentAction != null)
		{
			preShowTime += battleController.CurrentAction.GetAttackTime() * Settings.PreQueueActionTimePercentage;
		}

		bool shouldPreShow = isPlayer && battleController.TimeLeftOfAction <= preShowTime;
		

		bool shouldPostShow = battleController.TimeSinceActionStarted <= Settings.PostPickHangTime &&
			battleController.CurrentAction != null;

		bool showUi = Settings.ShowStatsForAi || ForceShowComplexStats || ((shouldPreShow || shouldPostShow) && isPlayer);

		var attackerLocked = false;
		var targetLocked = false;
		var selectedAttacker = currentAgent.SelectedPart;
		var selectedTarget = Body.eBodyPartType.None;
		if (showUi)
		{
			if (shouldPostShow)
			{
				selectedAttacker = battleController.CurrentAction.AttackerPartType;
				selectedTarget = battleController.CurrentAction.TargetPartType;
				attackerLocked = true;
				targetLocked = true;
			}
			else
			{
				if (currentAgent.LockedAttacker != Body.eBodyPartType.None)
				{
					selectedAttacker = currentAgent.LockedAttacker;
					selectedTarget = currentAgent.SelectedPart;
					attackerLocked = true;
				}

				if (currentAgent.LockedTarget != Body.eBodyPartType.None)
				{
					selectedTarget = currentAgent.LockedTarget;
					targetLocked = true;
				}
			}
		}

		var showDpad = hasVaildAction && currentAgent.ControlType == Agent.eControlType.Player &&
						(!attackerLocked || !targetLocked) && shouldPreShow;
						
		DpadAnimatorContoller.SetShow(showDpad);
		if (showDpad)
		{
			Vector2 targetPos = currentAgent.Body.DPadGameTransform.position;

			if (attackerLocked)
			{
				targetPos = opponent.Body.DPadGameTransform.position;
			}
			DpadAnimatorContoller.JumpToPoint(targetPos);
		}

		currentAgent.Body.ShowStats(showUi, selectedAttacker, attackerLocked, isOurTurn:true, ForceShowComplexStats);
		opponent.Body.ShowStats(showUi, selectedTarget, targetLocked, isOurTurn:false, ForceShowComplexStats);

		if (currentAgent.ControlType == Agent.eControlType.Player &&
			((selectedAttacker != Body.eBodyPartType.None && selectedAttacker != LastSelectedAttacker) ||
			(selectedTarget != Body.eBodyPartType.None  && selectedTarget != LastSelectedTarget)))
		{
			AudioSource.PlayClipAtPoint(SelectedAudioClip, transform.position);
		}
		LastSelectedAttacker = selectedAttacker;
		LastSelectedTarget = selectedTarget;
	}
}