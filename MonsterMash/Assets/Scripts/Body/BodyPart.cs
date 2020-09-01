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
	[SerializeField] TextMesh HealthDeltaText;
	[SerializeField] Animator PartAnimator;
	
	[Space]
	[Header("Stat UI")]
	[SerializeField] GameObject StatBox;

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
		if (HealthDeltaText == null ||
			PartAnimator == null)
		{
			Debug.LogWarning("not all Ui is set up for body part", this);
			return;
		}
		HealthDeltaText.text = $"-{healthDelta}";
		PartAnimator.SetTrigger("ShowHealthDelta");
	}

	public override string ToString()
	{
		return $"health: {CurrentHealth} / {MaxHealth}";
	}

	public void ShowStats(bool show, bool select)
	{
		StatBox.SetActive(show && !select);
	}
}