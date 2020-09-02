using UnityEngine;
using UnityEngine.UI;
using System;

public class BodyPart : MonoBehaviour
{
    public BodyPartData PartData { get; private set; }
    public int Armour { get { return PartData.Armour; } }
    public int CurrentHealth
    {
        get { return PartData.HealthCurrent; }
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

    public void ApplyAttack(int damage)
    {
        int preHealth = CurrentHealth;
        int healthDelta = damage - Armour;

        healthDelta = Math.Max(healthDelta, 0);

        CurrentHealth -= healthDelta;
		CurrentHealth = Math.Max(CurrentHealth, 0);
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
		PartAnimator.SetTrigger("Hit");
		PartAnimator.SetBool("Dead", !IsAlive);
    }

    public override string ToString()
    {
        return $"health: {CurrentHealth} / {PartData.HealthMaximum}";
    }

    public virtual void ShowStats(bool show, bool selected, bool isOurTurn, bool forceComplex)
    {
    }

    public virtual void SetBodyPartData(BodyPartSpriteLookup bodyPartImageLookup, BodyPartData data, Body.eBodyPartType bodyPartType)
    {
        PartData = data;
		BodyPartImage.sprite = bodyPartImageLookup.GetBodyPartSprite(bodyPartType, data.MonsterType);
		PartAnimator.SetBool("Dead", !IsAlive);
    }
}