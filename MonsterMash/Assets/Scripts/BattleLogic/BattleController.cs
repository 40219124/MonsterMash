using UnityEngine;
using System;

public class BattleController : MonoBehaviour
{
	public static BattleController Instance { get; private set;}
	
	public eBattleState BattleState { get; private set;}
	public enum eBattleState
	{
		NotInBattle,
		PlayerTurn,
		EnemyTurn,
		PlayerWon,
		EnemyWon
	}

	public Agent Player { get; private set;}
	public Agent Enemy { get; private set;}
	public float TurnTimeLeft { get; private set;}

	//current action
	public float TimeLeftOfAction { get; private set;}
	Action CurrentAction;

	void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError($"Instance already set");
		}
		Instance = this;
	}

	public void SetupBattle(Agent player, Agent enemy)
	{
		Debug.Log($"starting new Battle");
		Player = player;
		Enemy = enemy;
		BattleState = eBattleState.PlayerTurn;
		Player.OnGameStart(Enemy);
		Enemy.OnGameStart(Player);
		StartTurn();
	}

	void Update()
	{

		if (BattleState != eBattleState.PlayerTurn &&
			BattleState != eBattleState.EnemyTurn)
		{
			return;
		}

		TurnTimeLeft -= Time.deltaTime;
		TimeLeftOfAction -= Time.deltaTime;

		TurnTimeLeft = Math.Max(TurnTimeLeft, 0);
		TimeLeftOfAction = Math.Max(TimeLeftOfAction, 0);

		if (TimeLeftOfAction <= 0 &&
			CurrentAction != null)
		{
			FinishAction();
		}

		if (!Player.Body.IsAlive())
		{
			BattleState = eBattleState.EnemyWon;
			Debug.Log($"game over {BattleState}");
			return;
		}

		if(!Enemy.Body.IsAlive())
		{
			BattleState = eBattleState.PlayerWon;
			Debug.Log($"game over {BattleState}");
			return;
		}

		if (TurnTimeLeft <= 0 && TimeLeftOfAction <= 0)
		{
			StartTurn();
		}
	}

	void StartTurn()
	{
		TurnTimeLeft = Settings.TurnTime;
		TimeLeftOfAction = 0;

		switch (BattleState)
		{
			case eBattleState.PlayerTurn:
			{
				BattleState = eBattleState.EnemyTurn;
				break;
			}
			case eBattleState.EnemyTurn:
			{
				BattleState = eBattleState.PlayerTurn;
				break;
			}
			default:
			{
				Debug.LogError($"not handled state: {BattleState}");
				break;
			}
		}

		Player.OnTurnStart(BattleState == eBattleState.PlayerTurn);
		Enemy.OnTurnStart(BattleState == eBattleState.EnemyTurn);

		Debug.Log($"starting new turn: {BattleState}");
	}

	public bool TryAction(Action action)
	{
		if (BattleState != eBattleState.PlayerTurn &&
			BattleState != eBattleState.EnemyTurn)
		{
			Debug.LogError($"trying to do action while BattleState not a player turn: {BattleState}");
			return false;
		}

		if (TimeLeftOfAction > 0 && CurrentAction != null)
		{
			Debug.LogError($"trying to do action while still doing the last one TimeLeftOfAction: {TimeLeftOfAction}, CurrentAction {CurrentAction}");
			return false;
		}

		if (action == null)
		{
			Debug.LogError($"trying to do action with action that is null");
			return false;
		}

		if (action.AttackLimb == null)
		{
			Debug.LogError($"trying to do action with action.AttackLimb that is null");
			return false;
		}

		if (action.TargetBodyPart == null)
		{
			Debug.LogError($"trying to do action with action.TargetBodyPart that is null");
			return false;
		}

		if (!action.AttackLimb.IsAlive)
		{
			Debug.LogError($"trying to do action with attack limb({action.AttackLimb}) that not Alive");
			return false;
		}

		if (!action.TargetBodyPart.IsAlive)
		{
			Debug.LogError($"trying to do action with TargetBodyPart({action.TargetBodyPart}) that not Alive");
			return false;
		}

		//todo check that body part if part of the correct body

		int actionTime = action.AttackLimb.AttackTime;
		if (TurnTimeLeft + Settings.ActionTimeForgiveness <= action.AttackLimb.AttackTime)
		{
			Debug.Log($"not enough time to do action: {TurnTimeLeft} + {Settings.ActionTimeForgiveness} <= {action.AttackLimb.AttackTime}");
			return false;
		}

		//this is a valid action so lets do it yay!
		Debug.Log($"Doing Action: {action}");

		TimeLeftOfAction = action.AttackLimb.AttackTime;
		CurrentAction = action;
		return true;
	}

	public void FinishAction()
	{
		CurrentAction.TargetBodyPart.ApplyAttack(CurrentAction.AttackLimb.Damage);
		CurrentAction = null;
	}
}