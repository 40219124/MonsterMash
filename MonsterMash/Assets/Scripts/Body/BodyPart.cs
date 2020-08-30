using UnityEngine;
using System;

public class BodyPart : MonoBehaviour
{
	public eBodyPartType PartType;
	public int Armour;
	public int MaxHealth;
	public int CurrentHealth { get; private set;}
	public bool IsAlive { get {return CurrentHealth > 0;}}

	public enum eBodyPartType
	{
		None,
		Arm,
		Leg,
		Torso,
	}

	void Awake()
	{
		CurrentHealth = MaxHealth;
	}

	public void ApplyAttack(int damage)
	{
		int preHealth = CurrentHealth;
		int healthDelta = damage - Armour;
		
		healthDelta = Math.Max(healthDelta, 0);

		CurrentHealth -= healthDelta;

		Debug.Log($"ApplyAttack({damage}) health: {preHealth} -> {CurrentHealth}");
	}

	public override string ToString()
	{
		return $"health: {CurrentHealth} / {MaxHealth}";
	}
}