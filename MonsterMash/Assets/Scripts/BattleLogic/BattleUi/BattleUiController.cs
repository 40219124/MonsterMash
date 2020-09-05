using UnityEngine;
using UnityEngine.UI;
using System;

public class BattleUiController: MonoBehaviour
{
	[SerializeField] Animator BattleUiAnimator;
	[SerializeField] DpadAnimator DpadAnimatorContoller;
	[SerializeField] GameObject ButtonAPrompt;

	bool ForceShowComplexStats;

	void LateUpdate()
	{
		var battleController = BattleController.Instance;
		
		ButtonAPrompt.SetActive(battleController.BattleState == BattleController.eBattleState.BattleIntro);

		if (battleController.BattleState == BattleController.eBattleState.BattleIntro)
		{
			DpadAnimatorContoller.SetShow(false);
			battleController.Player.Body.ShowStats(true, Body.eBodyPartType.None, false, isOurTurn:false, true);
			battleController.Enemy.Body.ShowStats(true, Body.eBodyPartType.None, false, isOurTurn:false, true);
			return;
		}


		var currentAgent = battleController.CurrentAgent;
		var opponent = currentAgent.Opponent;

		bool isPlayer = Settings.ShowStatsForAi || currentAgent.ControlType == Agent.eControlType.Player;

		if (SimpleInput.GetInputState(EInput.Select) == EButtonState.Pressed)
		{
			ForceShowComplexStats = !ForceShowComplexStats;
		}

		bool shouldPreShow = isPlayer && battleController.TimeLeftOfAction <= Settings.PreShowBattleUiTime;

		bool shouldPostShow = battleController.TimeSinceActionStarted <= Settings.PostPickHangTime &&
			battleController.CurrentAction != null;

		bool showUi = ForceShowComplexStats || ((shouldPreShow || shouldPostShow) && isPlayer);

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

		var showDpad = currentAgent.ControlType == Agent.eControlType.Player;
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
	}
}