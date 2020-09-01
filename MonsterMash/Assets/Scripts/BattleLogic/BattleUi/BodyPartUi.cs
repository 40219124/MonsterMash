using UnityEngine;
using UnityEngine.UI;
using System;

public class BodyPartUi : MonoBehaviour
{
	[SerializeField] GameObject StatBoxRoot;

	[SerializeField] NumberRender HealthNumber;
	[SerializeField] NumberRender DamageNumber;
	[SerializeField] NumberRender ArmourNumber;
	[SerializeField] NumberRender TimeNumber;

	void Awake()
	{
		SetHealthNumber(false);
		SetDamageNumber(false);
		SetArmourNumber(false);
		SetTimeNumber(false);
		Show(false, false);
	}

	public void Show(bool show, bool selected)
	{
		StatBoxRoot.SetActive(show);
	}
	public void SetHealthNumber(bool show, int value=69)
	{
		HealthNumber.SetNumber(value);
	}
	public void SetDamageNumber(bool show, int value=69)
	{
		DamageNumber.SetNumber(value);
	}
	public void SetArmourNumber(bool show, int value=69)
	{
		ArmourNumber.SetNumber(value);
	}
	public void SetTimeNumber(bool show, int value=69)
	{
		TimeNumber.SetNumber(value);
	}
}