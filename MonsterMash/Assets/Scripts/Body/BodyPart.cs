using UnityEngine;
using UnityEngine.UI;
using System;

public class BodyPart : MonoBehaviour
{
	[Header("BattleStats")]
	public eBodyPartSlotType PartType;
	public int Armour;
	public int MaxHealth;
	public int CurrentHealth { get; private set;}
	public bool IsAlive { get {return CurrentHealth > 0;}}

	[Space]
	[Header("UI")]
	[SerializeField] NumberRender HealthDeltaNumber;
	[SerializeField] Animator PartAnimator;
	
	[Space]
	[Header("Stat UI")]
	[SerializeField] protected BodyPartUi StatBox;

	public enum eBodyPartSlotType
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

		//now trigger the UI
		if (HealthDeltaNumber == null ||
			PartAnimator == null)
		{
			Debug.LogWarning("not all Ui is set up for body part", this);
			return;
		}
		HealthDeltaNumber.SetNumber(healthDelta);
		PartAnimator.SetTrigger("ShowHealthDelta");
	}

	public override string ToString()
	{
		return $"health: {CurrentHealth} / {MaxHealth}";
	}

	public virtual void ShowStats(bool show, bool selected)
	{
		bool shouldShow = show && IsAlive;
		StatBox.Show(shouldShow, selected);
	}
}