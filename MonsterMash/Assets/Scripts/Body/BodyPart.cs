using UnityEngine;
using UnityEngine.UI;
using System;

public class BodyPart : MonoBehaviour
{
	public Body.eBodyPartType BodyPartType { get; private set; }
    public BodyPartData PartData { get; private set; }
    public int Armour { get { return PartData == null? 0 : PartData.Armour; } }
    public int CurrentHealth
    {
        get { return PartData == null? 0: PartData.HealthCurrent; }
        private set { PartData.HealthCurrent = value; }
    }
    public bool IsAlive { get { return CurrentHealth > 0; } }
	

	[SerializeField] SpriteRenderer BodyPartImage;
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
		if (BodyPartImage == null ||
			HealthDeltaNumber == null ||
			StatBox == null ||
            PartAnimator == null)
        {
            MMLogger.LogWarning("not all Ui is set up for body part", this);
            return;
        }

		HealthDeltaNumber.UseLargeNumbers = true;
	}

    public void ApplyAttack(int damage)
    {
        int preHealth = CurrentHealth;
        int healthDelta = damage - Armour;

        healthDelta = Math.Max(healthDelta, 0);

        CurrentHealth -= healthDelta;
		CurrentHealth = Math.Max(CurrentHealth, 0);
        MMLogger.Log($"ApplyAttack({damage}) health: {preHealth} -> {CurrentHealth}");

        HealthDeltaNumber.SetNumber(healthDelta);

        PartAnimator.SetTrigger("ShowHealthDelta");
		PartAnimator.SetTrigger("Hit");
		PartAnimator.SetBool("Dead", !IsAlive);
    }

    public override string ToString()
    {
        return $"health: {CurrentHealth} / {PartData.HealthMaximum}";
    }

    public virtual void ShowStats(bool show, bool selected, bool isOurTurn, bool forceComplex, bool forceDisable=false)
    {
    }

    public virtual void SetBodyPartData(BodyPartSpriteLookup bodyPartImageLookup, BodyPartData data, Body.eBodyPartType bodyPartType)
    {
		BodyPartType = bodyPartType;
        PartData = data;

		SetSprite(bodyPartImageLookup);

		PartAnimator.SetBool("Dead", !IsAlive);
    }

	public void SetSprite(BodyPartSpriteLookup partSpriteLookup, EMonsterType monsterType=EMonsterType.none)
	{
		if (monsterType == EMonsterType.none)
		{
			if(PartData == null)
			{
				BodyPartImage.sprite = null;
				return;
			}
			monsterType = PartData.MonsterType;
		}

		BodyPartImage.sprite = partSpriteLookup.GetBodyPartSprite(BodyPartType, monsterType);
	}
}