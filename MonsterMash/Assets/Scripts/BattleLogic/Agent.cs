using UnityEngine;
using System.Collections.Generic;

public class Agent : MonoBehaviour
{
	protected bool IsOurTurn { get; private set;}
	public Agent Opponent { get; private set;}

	public Body Body;
	
	public eControlType ControlType;
	public enum eControlType
	{
		None,
		Player,
		Ai
	}
	
	public Body.eBodyPartType SelectedPart { get; private set;}

	public Body.eBodyPartType LockedAttacker { get; private set;}
	public Body.eBodyPartType LockedTarget { get; private set;}

	void Update()
	{
		var battleController = BattleController.Instance;
		if (!(IsOurTurn && 
			battleController.TimeLeftOfAction <= 0 &&
			(battleController.BattleState == BattleController.eBattleState.PlayerTurn ||
			battleController.BattleState == BattleController.eBattleState.EnemyTurn)))
		{
			return;
		}

		if (ControlType == eControlType.Player)
		{
			GetPlayerAction();
		}
		else if (ControlType == eControlType.Ai)
		{
			GetAiAction();
		}
		
		if (LockedAttacker != Body.eBodyPartType.None &&
			LockedTarget != Body.eBodyPartType.None)
		{
			var action = new Action(Body, LockedAttacker, Opponent.Body, LockedTarget);
			BattleController.Instance.TryAction(action);
			LockedAttacker = Body.eBodyPartType.None;
			LockedTarget = Body.eBodyPartType.None;
			AiPickedTarget = Body.eBodyPartType.None;
			SelectedPart = Body.eBodyPartType.None;
		}
	}

	bool CanUseLimb(Limb limb)
	{
		return limb.IsAlive && limb.AttackTime < BattleController.Instance.TurnTimeLeft + Settings.ActionTimeForgiveness;
	}

	void GetPlayerAction()
	{
		float horizontalValue = Input.GetAxisRaw("Horizontal");
		float verticalValue = Input.GetAxisRaw("Vertical");
		float aButton = Input.GetAxisRaw("ButtonA");

		if (horizontalValue > 0 && verticalValue == 0 && CanUseLimb(Body.RightArmPart))
		{
			SelectedPart = Body.eBodyPartType.RightArm;
		}
		else if (horizontalValue < 0 && verticalValue == 0 && CanUseLimb(Body.LeftArmPart))
		{
			SelectedPart = Body.eBodyPartType.LeftArm;
		}
		else if (horizontalValue == 0 && verticalValue < 0 && CanUseLimb(Body.LegsPart))
		{
			SelectedPart = Body.eBodyPartType.Leg;
		}
		else if (horizontalValue == 0 && verticalValue > 0 && LockedAttacker != Body.eBodyPartType.None)
		{
			SelectedPart = Body.eBodyPartType.Torso;
		}

		if (aButton > 0)
		{
			if (LockedAttacker == Body.eBodyPartType.None)
			{
				LockedAttacker = SelectedPart;
			}
			else
			{
				LockedTarget = SelectedPart;
			}
			SelectedPart = Body.eBodyPartType.None;
		}
	}

	public int SortBylimbAscending(Limb limb1, Limb limb2)
    {
		var damage1 = limb1.Damage;
		var damage2 = limb2.Damage;
		if (damage1 == damage2)
		{
			var time1 = limb1.AttackTime;
			var time2 = limb2.AttackTime;
			return time1.CompareTo(time2);
		}
		else
		{
			return damage1.CompareTo(damage2);
		}
    }


	Body.eBodyPartType AiPickedTarget;
	float PickAttackerTime = 0;
	float PickTargetTime = 0;
	void GetAiAction()
	{
		if(AiPickedTarget == Body.eBodyPartType.None)
		{
			SelectedPart = CalBestAction();
			if (SelectedPart == Body.eBodyPartType.None ||
				AiPickedTarget == Body.eBodyPartType.None)
			{
				SelectedPart = Body.eBodyPartType.None;
				AiPickedTarget = Body.eBodyPartType.None;
			}
			Debug.Log($"AI Picked action attacker: {SelectedPart} target {AiPickedTarget}");
			PickAttackerTime = Random.Range(Settings.AiPickAttackerMinTime, Settings.AiPickAttackerMaxTime);
			PickTargetTime = Random.Range(Settings.AiPickTargetMinTime, Settings.AiPickTargetMaxTime);
		}
		else
		{
			if (LockedAttacker == Body.eBodyPartType.None)
			{
				PickAttackerTime -= Time.deltaTime;
				if(PickAttackerTime <= 0)
				{
					LockedAttacker = SelectedPart;
					SelectedPart = AiPickedTarget;
				}
			}
			else if (LockedTarget == Body.eBodyPartType.None)
			{
				PickTargetTime -= Time.deltaTime;
				if(PickTargetTime <= 0)
				{
					LockedTarget = SelectedPart;
				}
			}
		}
	}

	Body.eBodyPartType CalBestAction()
	{
		var attackLimbList = new List<(Limb, Body.eBodyPartType)>(3);
		attackLimbList.Add((Body.LeftArmPart, Body.eBodyPartType.LeftArm));
		attackLimbList.Add((Body.RightArmPart, Body.eBodyPartType.RightArm));
		attackLimbList.Add((Body.LegsPart, Body.eBodyPartType.Leg));
		
		//sort my by damage then time
		attackLimbList.Sort((x,y)=>SortBylimbAscending(x.Item1, y.Item1));

		var bodyPartPrioList = new List<(BodyPart, Body.eBodyPartType)>(4);
		bodyPartPrioList.Add((Opponent.Body.TorsoPart, Body.eBodyPartType.Torso));

		//todo sort this by health + dps
		bodyPartPrioList.Add((Opponent.Body.LeftArmPart, Body.eBodyPartType.LeftArm));
		bodyPartPrioList.Add((Opponent.Body.RightArmPart, Body.eBodyPartType.RightArm));
		bodyPartPrioList.Add((Opponent.Body.LegsPart, Body.eBodyPartType.Leg));

		var pickedAttacker = Body.eBodyPartType.None;
		
		foreach (var tBodyPart in bodyPartPrioList)
		{
			var bodyPart = tBodyPart.Item1;
			if (!bodyPart.IsAlive)
			{
				continue;
			}
			
			foreach (var tAttacker in attackLimbList)
			{
				if (tAttacker.Item1.Damage - tBodyPart.Item1.Armour >= tBodyPart.Item1.CurrentHealth)
				{
					pickedAttacker = tAttacker.Item2;
					AiPickedTarget = tBodyPart.Item2;
				}
			}
			if(AiPickedTarget != Body.eBodyPartType.None)
			{
				break;
			}
		}

		if(AiPickedTarget == Body.eBodyPartType.None)
		{
			float lowestHealth = float.MaxValue;
			int targetArmour = 0;
			foreach (var tBodyPart in bodyPartPrioList)
			{
				var bodyPart = tBodyPart.Item1;
				if (!bodyPart.IsAlive)
				{
					continue;
				}

				if (tBodyPart.Item1.CurrentHealth < lowestHealth)
				{
					AiPickedTarget = tBodyPart.Item2;
					lowestHealth = tBodyPart.Item1.CurrentHealth;
					targetArmour = tBodyPart.Item1.Armour;
				}
			}

			float bestDps = 0;
			foreach (var tAttacker in attackLimbList)
			{
				if (!tAttacker.Item1.IsAlive)
				{
					continue;
				}
				if (targetArmour >= tAttacker.Item1.Damage)
				{
					continue;
				}

				var numAttacks = Mathf.Floor(BattleController.Instance.TurnTimeLeft / tAttacker.Item1.AttackTime);
				var dps = tAttacker.Item1.Damage * numAttacks;
				if (dps > bestDps)
				{
					pickedAttacker = tAttacker.Item2;
					bestDps = dps;
				}
			}
		}
		return pickedAttacker;
	}
	
	public void OnGameStart(Agent opponent, MonsterProfile profile)
	{
		Opponent = opponent;
		Body.SetProfileData(profile);
	}

	public void OnTurnStart(bool isOurTurn)
	{
		IsOurTurn = isOurTurn;

		LockedAttacker = Body.eBodyPartType.None;
		LockedTarget = Body.eBodyPartType.None;
		AiPickedTarget = Body.eBodyPartType.None;
		SelectedPart = Body.eBodyPartType.None;
	}
}