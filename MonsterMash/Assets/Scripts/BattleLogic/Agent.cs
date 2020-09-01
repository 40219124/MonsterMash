using UnityEngine;
using System.Collections.Generic;

public class Agent : MonoBehaviour
{
	protected bool IsOurTurn { get; private set;}
	protected Agent Opponent { get; private set;}

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
		Body.ShowStats(battleController.TimeLeftOfAction <= Settings.PreShowBattleUiTime, SelectedPart, true);

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
			if (BattleController.Instance.TryAction(action))
			{
				LockedAttacker = Body.eBodyPartType.None;
				LockedTarget = Body.eBodyPartType.None;
			}
		}
	}

	void GetPlayerAction()
	{
		float horizontalValue = Input.GetAxisRaw("Horizontal");
		float verticalValue = Input.GetAxisRaw("Vertical");
		float aButton = Input.GetAxisRaw("ButtonA");

		if (horizontalValue > 0 && verticalValue == 0)
		{
			SelectedPart = Body.eBodyPartType.RightArm;
		}
		else if (horizontalValue < 0 && verticalValue == 0)
		{
			SelectedPart = Body.eBodyPartType.LeftArm;
		}
		else if (horizontalValue == 0 && verticalValue < 0)
		{
			SelectedPart = Body.eBodyPartType.Leg;
		}
		else if (horizontalValue == 0 && horizontalValue < 0 && LockedAttacker != Body.eBodyPartType.None)
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

	void GetAiAction()
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
					LockedAttacker = tAttacker.Item2;
					LockedTarget = tBodyPart.Item2;
				}
			}
			if(LockedTarget != Body.eBodyPartType.None)
			{
				break;
			}
		}

		if(LockedTarget == Body.eBodyPartType.None)
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
					LockedTarget = tBodyPart.Item2;
					lowestHealth = tBodyPart.Item1.CurrentHealth;
					targetArmour = tBodyPart.Item1.Armour;
				}
			}

			float bestDps = 0;
			foreach (var tAttacker in attackLimbList)
			{
				if (targetArmour >= tAttacker.Item1.Damage)
				{
					continue;
				}

				var numAttacks = Mathf.Floor(BattleController.Instance.TurnTimeLeft / tAttacker.Item1.AttackTime);
				var dps = tAttacker.Item1.Damage * numAttacks;
				if (dps > bestDps)
				{
					LockedAttacker = tAttacker.Item2;
					bestDps = dps;
				}
			}
		}
	}
	
	public void OnGameStart(Agent opponent)
	{
		Opponent = opponent;
	}

	public void OnTurnStart(bool isOurTurn)
	{
		IsOurTurn = isOurTurn;
		LockedAttacker = Body.eBodyPartType.None;
		LockedTarget = Body.eBodyPartType.None;
	}
}