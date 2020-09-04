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
		if (!IsOurTurn || 
			!(battleController.BattleState == BattleController.eBattleState.PlayerTurn ||
			battleController.BattleState == BattleController.eBattleState.EnemyTurn ||
			battleController.BattleState == BattleController.eBattleState.TurnTransition))
		{
			return;
		}

		if ((LockedAttacker == Body.eBodyPartType.None ||
			LockedTarget == Body.eBodyPartType.None) &&
			battleController.TimeLeftOfAction <= Settings.PreQueueActionTime)
		{
			if (ControlType == eControlType.Player)
			{
				GetPlayerAction();
			}
			else if (ControlType == eControlType.Ai &&
				battleController.TimeLeftOfAction <= 0)
			{
				GetAiAction();
			}
		}
		
		if (LockedAttacker != Body.eBodyPartType.None &&
			LockedTarget != Body.eBodyPartType.None &&
			battleController.TimeLeftOfAction <= 0 &&
			(battleController.BattleState == BattleController.eBattleState.PlayerTurn ||
			battleController.BattleState == BattleController.eBattleState.EnemyTurn))
		{
			var action = new Action(Body, LockedAttacker, Opponent.Body, LockedTarget);
			BattleController.Instance.TryAction(action);
			LockedAttacker = Body.eBodyPartType.None;
			LockedTarget = Body.eBodyPartType.None;
			AiPickedTarget = Body.eBodyPartType.None;
			SelectedPart = Body.eBodyPartType.None;
		}
	}

	void GetPlayerAction()
	{
		var mostRecentDpad = SimpleInput.GetRecentDpad();

		if (SimpleInput.GetInputActive(mostRecentDpad))
		{

			switch (mostRecentDpad)
			{
				case (EInput.dpadUp):
				{
					if (LockedAttacker != Body.eBodyPartType.None)
					{
						SelectedPart = Body.eBodyPartType.Torso;
					}
					break;
				}
				case (EInput.dpadLeft):
				{
					if (Body.LeftArmPart.IsValidAttacker() || LockedAttacker != Body.eBodyPartType.None)
					{
						SelectedPart = Body.eBodyPartType.LeftArm;
					}
					break;
				}
				case (EInput.dpadRight):
				{
					if (Body.RightArmPart.IsValidAttacker() || LockedAttacker != Body.eBodyPartType.None)
					{
						SelectedPart = Body.eBodyPartType.RightArm;
					}
					break;
				}
				case (EInput.dpadDown):
				{
					if (Body.LegsPart.IsValidAttacker() || LockedAttacker != Body.eBodyPartType.None)
					{
						SelectedPart = Body.eBodyPartType.Leg;
					}
					break;
				}
				default:
				{
					Debug.LogError($" unexpected button: {mostRecentDpad}");
					return;
				}
			}
		}
		
		if ((!Settings.DpadOnlyCombat && SimpleInput.GetInputState(EInput.A) == EButtonState.Pressed) ||
			(Settings.DpadOnlyCombat && SimpleInput.GetInputState(mostRecentDpad) == EButtonState.Released))
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

		if (SimpleInput.GetInputState(EInput.B) == EButtonState.Pressed)
		{
			if (LockedTarget != Body.eBodyPartType.None)
			{
				LockedTarget = Body.eBodyPartType.None;
			}
			else
			{
				LockedAttacker = Body.eBodyPartType.None;
			}
			SelectedPart = Body.eBodyPartType.None;
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
				Debug.Log($"AI Picked action but one none attacker: \"{SelectedPart}\" target \"{AiPickedTarget}\"");
				SelectedPart = Body.eBodyPartType.None;
				AiPickedTarget = Body.eBodyPartType.None;
			}
			else
			{
				Debug.Log($"AI Picked action attacker: \"{SelectedPart}\" target \"{AiPickedTarget}\"");
				PickAttackerTime = Random.Range(Settings.AiPickAttackerMinTime, Settings.AiPickAttackerMaxTime);
				PickTargetTime = Random.Range(Settings.AiPickTargetMinTime, Settings.AiPickTargetMaxTime);
			}
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

	public int SortByLimbAscending(Limb limb1, Limb limb2)
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

	Body.eBodyPartType CalBestAction()
	{
		var attackLimbList = new List<(Limb, Body.eBodyPartType)>(3);
		attackLimbList.Add((Body.LeftArmPart, Body.eBodyPartType.LeftArm));
		attackLimbList.Add((Body.RightArmPart, Body.eBodyPartType.RightArm));
		attackLimbList.Add((Body.LegsPart, Body.eBodyPartType.Leg));
		
		//sort my by damage then time
		attackLimbList.Sort((x,y)=>SortByLimbAscending(x.Item1, y.Item1));

		var bodyPartPrioList = new List<(BodyPart, Body.eBodyPartType)>(4);
		bodyPartPrioList.Add((Opponent.Body.TorsoPart, Body.eBodyPartType.Torso));

		//todo sort this by health + dps
		bodyPartPrioList.Add((Opponent.Body.LeftArmPart, Body.eBodyPartType.LeftArm));
		bodyPartPrioList.Add((Opponent.Body.RightArmPart, Body.eBodyPartType.RightArm));
		bodyPartPrioList.Add((Opponent.Body.LegsPart, Body.eBodyPartType.Leg));

		var pickedAttacker = Body.eBodyPartType.None;
		AiPickedTarget = Body.eBodyPartType.None;
		int bestSecondsToKill = int.MaxValue;

		foreach (var tBodyPart in bodyPartPrioList)
		{
			var bodyPart = tBodyPart.Item1;
			if (!bodyPart.IsAlive)
			{
				continue;
			}
			
			foreach (var tAttacker in attackLimbList)
			{
				if (!tAttacker.Item1.IsAlive ||
					tAttacker.Item1.AttackTime >= BattleController.Instance.TurnTimeLeft)
				{
					continue;
				}

				int healthDelta = tAttacker.Item1.Damage - tBodyPart.Item1.Armour;
				if (healthDelta <= 0)
				{
					continue;
				}
				
				int secondsToKill = tBodyPart.Item1.CurrentHealth / healthDelta;
				if (bestSecondsToKill >= secondsToKill)
				{
					pickedAttacker = tAttacker.Item2;
					AiPickedTarget = tBodyPart.Item2;
					bestSecondsToKill = secondsToKill;
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