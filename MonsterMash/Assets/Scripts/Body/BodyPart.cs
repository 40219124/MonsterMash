using UnityEngine;
using UnityEngine.UI;
using System;

public class BodyPart : MonoBehaviour
{
    public BodyPartData PartData { get; private set; }

    public eBodyPartSlotType PartType { get { return PartData.BodyPartType; } }
    public int Armour { get { return PartData.Armour; } }
    public int MaxHealth { get { return PartData.HealthMaximum; } }
    public int CurrentHealth
    {
        get { return PartData.HealthCurrent; }
        private set { PartData.HealthCurrent = value; }
    }
    public bool IsAlive { get { return CurrentHealth > 0; } }

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
    }

    public override string ToString()
    {
        return $"health: {CurrentHealth} / {MaxHealth}";
    }

    public virtual void ShowStats(bool show, bool selected, bool isOurTurn)
    {
        bool shouldShow = show && IsAlive;
        StatBox.Show(shouldShow, selected, selected, !IsAlive);
    }

    public virtual void SetBodyPartData(BodyPartData data)
    {
        PartData = data;
    }
}