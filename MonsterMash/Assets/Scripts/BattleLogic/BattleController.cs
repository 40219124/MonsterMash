using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
	public static BattleController Instance { get; private set; }
	[SerializeField] CameraShake CameraShakeController;


	public eBattleState BattleState { get; private set; }
	public enum eBattleState
	{
		NotInBattle,
		BattleIntro,
		PlayerTurn,
		EnemyTurn,
		TurnTransition,
		PlayerWon,
		EnemyWon
	}

	public Agent Player;
	public Agent Enemy;
	public float TurnTimeLeft { get; private set; }

	//current action
	public float TimeLeftOfAction { get; private set; }
	public float TimeSinceActionStarted { get; private set; }
	public float TurnTransitionTimeLeft { get; private set; }
	public Action CurrentAction { get; private set; }
	public Agent CurrentAgent { get; private set; }

	void Awake()
	{
		if (Instance != null)
		{
			MMLogger.LogError($"Instance already set");
		}
		Instance = this;
	}

	public void SetupBattle(MonsterProfile playerProfile, MonsterProfile enemyProfile)
	{
		MMLogger.Log($"starting new Battle");
		Player.OnGameStart(Enemy, playerProfile);
		Enemy.OnGameStart(Player, enemyProfile);
		BattleState = eBattleState.BattleIntro;
		TimeSinceActionStarted = 0;

		CurrentAgent = UnityEngine.Random.Range(0, 2) == 1? Player : Enemy;

		if (OverworldMemory.PlayerProfile != null)
		{
			OverworldMemory.PlayerProfile.ProfileStats[eStatType.NumberOfBattlesPlayed].Value += 1;
		}
	}

	public bool HasValidAttcker(Agent agent)
	{
		return agent.Body.LeftArmPart.IsValidAttacker() ||
				agent.Body.RightArmPart.IsValidAttacker() ||
				agent.Body.LegsPart.IsValidAttacker();
	}

	void Update()
	{
		TimeSinceActionStarted += Time.deltaTime;

		if (BattleState == eBattleState.BattleIntro)
		{
			float introMaxTime = Settings.BattleIntroMaxTime;

			if (Player.ControlType == Agent.eControlType.Ai &&
				Enemy.ControlType == Agent.eControlType.Ai)
			{
				introMaxTime = 2.0f;
			}

			if ( (introMaxTime >= 0 && TimeSinceActionStarted >= introMaxTime) ||
				SimpleInput.GetInputState(EInput.A) == EButtonState.Released)
			{
				StartTurnTransition();
			}
		}

		if (BattleState == eBattleState.TurnTransition)
		{
			TurnTransitionTimeLeft -= Time.deltaTime;
			if (TurnTransitionTimeLeft <= 0)
			{
				StartTurn();
			}
		}

		if (BattleState != eBattleState.PlayerTurn &&
			BattleState != eBattleState.EnemyTurn)
		{
			if (SimpleInput.GetInputState(EInput.A) == EButtonState.Released)
			{
				MMLogger.Log($"pressed A now leaving game over to go to next screen");
				
				if (BattleState == eBattleState.PlayerWon)
				{
					// Remove enemy from memory 
					bool dropLoot = UnityEngine.Random.Range(0.0f, 1.0f) < Settings.ChanceOfALimbDrop;
					if (OverworldMemory.PlayerProfile.ProfileStats[eStatType.NumberOfBattlesPlayed].Value <= 1)
					{
						dropLoot = true;
					}
					
					OverworldMemory.OpponentBeaten(dropLoot);
					if(dropLoot || Settings.AlwaysGoToPickerPostBattle)
					{
						FlowManager.Instance.TransToPicker(Settings.SceneBattle);
					}
					else
					{
						FlowManager.Instance.TransToOverworld(Settings.SceneBattle);
					}
				}
				else if (BattleState == eBattleState.EnemyWon)
				{
					FlowManager.Instance.TransToGameOver(Settings.SceneBattle, false);
				}

			}
			return;
		}

		var deltaTime = Time.deltaTime;

		if (CurrentAgent.ControlType != Agent.eControlType.Player)
		{
			deltaTime *= Settings.AiTurnTimeSpeedMultiplier;
		}

		TimeLeftOfAction -= deltaTime;
		TimeLeftOfAction = Math.Max(TimeLeftOfAction, 0);

		if (TimeLeftOfAction <= 0 && CurrentAction == null)
		{
			if (!HasValidAttcker(CurrentAgent))
			{
				deltaTime *= Settings.NoActionAvailableSpeedMultiplier;
			}
		}
		TurnTimeLeft -= deltaTime;
		TurnTimeLeft = Math.Max(TurnTimeLeft, 0);

		if (!Player.Body.IsAlive())
		{
			BattleState = eBattleState.EnemyWon;
			MMLogger.Log($"game over {BattleState} now in game over state");
			Player.Body.PlayDeathAnim();
			return;
		}

		if (!Enemy.Body.IsAlive())
		{
			BattleState = eBattleState.PlayerWon;
			MMLogger.Log($"game over {BattleState} now in game over state");
			Enemy.Body.PlayDeathAnim();
			return;
		}

		if (TurnTimeLeft <= 0 && TimeLeftOfAction <= 0)
		{
			StartTurnTransition();
		}
	}

	void StartTurnTransition()
	{
		if (BattleState != eBattleState.BattleIntro)
		{
			if (CurrentAgent == Player)
			{
				CurrentAgent = Enemy;
			}
			else
			{
				CurrentAgent = Player;
			}
		}
		Player.OnTurnStart(CurrentAgent == Player);
		Enemy.OnTurnStart(CurrentAgent != Player);

		TurnTransitionTimeLeft = Settings.TurnTransitionTime;
		BattleState = eBattleState.TurnTransition;
	}

	void StartTurn()
	{
		TurnTimeLeft = Settings.TurnTime;
		TimeLeftOfAction = 0;
		TimeSinceActionStarted = 0;

		if (CurrentAgent == Player)
		{
			if (OverworldMemory.PlayerProfile != null)
			{
				OverworldMemory.PlayerProfile.ProfileStats[eStatType.TurnsPlayedInBattle].Value += 1;
			}
			BattleState = eBattleState.PlayerTurn;
		}
		else
		{
			BattleState = eBattleState.EnemyTurn;
		}

		MMLogger.Log($"starting new turn: {BattleState}");
	}

	public bool TryAction(Action action)
	{
		if (BattleState != eBattleState.PlayerTurn &&
			BattleState != eBattleState.EnemyTurn)
		{
			MMLogger.LogError($"trying to do action while BattleState not a player turn: {BattleState}");
			return false;
		}

		if (TimeLeftOfAction > 0 && CurrentAction != null)
		{
			MMLogger.LogError($"trying to do action while still doing the last one TimeLeftOfAction: {TimeLeftOfAction}, CurrentAction {CurrentAction}");
			return false;
		}

		if (action == null)
		{
			MMLogger.LogError($"trying to do action with action that is null");
			return false;
		}

		var attacker = action.Attacker;
		var target = action.Target;

		if (attacker == null)
		{
			MMLogger.LogError($"trying to do action with action.Attacker that is null");
			return false;
		}

		if (target == null)
		{
			MMLogger.LogError($"trying to do action with action.Target that is null");
			return false;
		}

		var attackerLimb = action.Attacker.GetLimb(action.AttackerPartType);
		var targetBodyPart = action.Target.GetBodyPart(action.TargetPartType);

		if (attackerLimb == null)
		{
			MMLogger.LogError($"trying to do action with attackerLimb that is null: {action}");
			return false;
		}

		if (targetBodyPart == null)
		{
			MMLogger.LogError($"trying to do action with targetBodyPart that is null: {action}");
			return false;
		}

		if (!attackerLimb.IsAlive)
		{
			MMLogger.LogError($"trying to do action with attack limb({attackerLimb}) that not Alive");
			return false;
		}

		if (!targetBodyPart.IsAlive)
		{
			MMLogger.LogError($"trying to do action with TargetBodyPart({targetBodyPart}) that not Alive");
			return false;
		}

		//todo check that body part if part of the correct body

		int actionTime = attackerLimb.AttackTime;
		if (TurnTimeLeft + Settings.ActionTimeForgiveness <= actionTime)
		{
			MMLogger.Log($"not enough time to do action: {TurnTimeLeft} + {Settings.ActionTimeForgiveness} <= {actionTime}");
			return false;
		}

		//this is a valid action so lets do it yay!
		MMLogger.Log($"Doing Action: {action}");

		TimeLeftOfAction = actionTime;
		TimeSinceActionStarted = 0;
		CurrentAction = action;
		StartCoroutine(DoAttack(actionTime, attackerLimb));
		return true;
	}

	IEnumerator DoAttack(int actionTime, Limb attackerLimb)
	{
		CurrentAgent.Body.StartAttack();

		yield return new WaitForSeconds(actionTime / 2);

		int damage = attackerLimb.Damage;
		CurrentAction.Target.ApplyAttack(CurrentAction.TargetPartType, damage);

		float shakePower = 0;
		if (damage < 5f)
		{
			shakePower = 5f;
		}
		else if (damage >= 5f && damage < 15f)
		{
			shakePower = 10f;
		}
		else
		{
			shakePower = 15f;
		}
		CameraShakeController.PlayShake(shakePower);
		CurrentAgent.Body.EndAttack();

		while (TimeLeftOfAction > 0)
		{
			yield return null;
		}

		CurrentAction = null;
	}
}