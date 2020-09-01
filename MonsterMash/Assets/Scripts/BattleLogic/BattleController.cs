using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
	public static BattleController Instance { get; private set;}

	[SerializeField] RectTransform Vignette;
	[SerializeField] AnimationCurve VignetteCurve;
	[SerializeField] CameraShake CameraShakeController;

	
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
	public float TimeSinceActionStarted { get; private set;}
	public Action CurrentAction { get; private set;}
	public Agent CurrentAgent { get; private set;}

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
		BattleState = eBattleState.EnemyTurn;
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
		TimeSinceActionStarted += Time.deltaTime;

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
		TimeSinceActionStarted = 0;

		switch (BattleState)
		{
			case eBattleState.PlayerTurn:
			{
				BattleState = eBattleState.EnemyTurn;
				CurrentAgent = Enemy;
				break;
			}
			case eBattleState.EnemyTurn:
			{
				BattleState = eBattleState.PlayerTurn;
				CurrentAgent = Player;
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

		var attacker = action.Attacker;
		var target = action.Target;

		if (attacker == null)
		{
			Debug.LogError($"trying to do action with action.Attacker that is null");
			return false;
		}

		if (target == null)
		{
			Debug.LogError($"trying to do action with action.Target that is null");
			return false;
		}

		var attackerLimb = action.Attacker.GetLimb(action.AttackerPartType);
		var targetBodyPart = action.Target.GetBodyPart(action.TargetPartType);

		if (attackerLimb == null)
		{
			Debug.LogError($"trying to do action with attackerLimb that is null");
			return false;
		}

		if (targetBodyPart == null)
		{
			Debug.LogError($"trying to do action with targetBodyPart that is null");
			return false;
		}

		if (!attackerLimb.IsAlive)
		{
			Debug.LogError($"trying to do action with attack limb({attackerLimb}) that not Alive");
			return false;
		}

		if (!targetBodyPart.IsAlive)
		{
			Debug.LogError($"trying to do action with TargetBodyPart({targetBodyPart}) that not Alive");
			return false;
		}

		//todo check that body part if part of the correct body

		int actionTime = attackerLimb.AttackTime;
		if (TurnTimeLeft + Settings.ActionTimeForgiveness <= actionTime)
		{
			Debug.Log($"not enough time to do action: {TurnTimeLeft} + {Settings.ActionTimeForgiveness} <= {actionTime}");
			return false;
		}

		//this is a valid action so lets do it yay!
		Debug.Log($"Doing Action: {action}");

		TimeLeftOfAction = actionTime;
		TimeSinceActionStarted = 0;
		CurrentAction = action;
		CurrentAgent.Body.StartAttack();
		return true;
	}

	public void FinishAction()
	{
		var attackerLimb = CurrentAction.Attacker.GetLimb(CurrentAction.AttackerPartType);
		int damage = attackerLimb.Damage;
		CurrentAction.Target.ApplyAttack(CurrentAction.TargetPartType, damage);
		CurrentAction = null;
		CurrentAgent.Body.EndAttack();
		//StartCoroutine(DoVignetteFlash());
		CameraShakeController.PlayShake(10f);
	}

	IEnumerator DoVignetteFlash()
	{
		if (VignetteCurve != null)
		{
			yield break;
		}
		float flashTime = 0.1f;

		Vignette.gameObject.SetActive(true);

		float timer = 0f;
		while (timer <= flashTime)
		{
			float size = VignetteCurve.Evaluate(timer/flashTime);
			Vignette.sizeDelta = new Vector2(size,size);

			yield return null;
			timer += Time.deltaTime;
		}

		Vignette.gameObject.SetActive(false);
	}
}