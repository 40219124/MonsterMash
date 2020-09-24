using UnityEngine;
using UnityEngine.UI;
using System;

public class BodyPartUi : MonoBehaviour
{
	[SerializeField] Animator StatAnimator;
	[SerializeField] GameObject StatBoxRoot;

	[SerializeField] GameObject HealthIconGameObject;
	[SerializeField] NumberRender HealthNumber;

	[SerializeField] GameObject DamageIconGameObject;
	[SerializeField] NumberRender DamageNumber;

	[SerializeField] GameObject ArmourIconGameObject;
	[SerializeField] NumberRender ArmourNumber;

	[SerializeField] GameObject TimeIconGameObject;
	[SerializeField] NumberRender TimeNumber;

	void Awake()
	{
		SetHealthNumber(false);
		SetDamageNumber(false);
		SetArmourNumber(false);
		SetTimeNumber(false);
		Show(false, false, true, false);
	}

	public void Show(bool show, bool selected, bool showComplex, bool disabled)
	{
		StatBoxRoot.SetActive(show);
		StatAnimator.SetBool("Complex", showComplex);
		StatAnimator.SetBool("Selected", selected);
		StatAnimator.SetBool("Disabled", disabled);
	}
	public void SetHealthNumber(bool show, int value=69)
	{
		HealthIconGameObject.SetActive(show);
		HealthNumber.gameObject.SetActive(show);
		HealthNumber.SetNumber(value);
	}
	public void SetDamageNumber(bool show, int value=69)
	{
		if (value == 0)
		{
			show = false;
		}
		DamageIconGameObject.SetActive(show);
		DamageNumber.gameObject.SetActive(show);
		DamageNumber.SetNumber(value);
	}
	public void SetArmourNumber(bool show, int value=69)
	{
		if (value == 0)
		{
			show = false;
		}
		ArmourIconGameObject.SetActive(show);
		ArmourNumber.gameObject.SetActive(show);
		ArmourNumber.SetNumber(value);
	}
	public void SetTimeNumber(bool show, int value=69)
	{
		if (value == 0)
		{
			show = false;
		}
		TimeIconGameObject.SetActive(show);
		TimeNumber.gameObject.SetActive(show);
		TimeNumber.SetNumber(value);
	}
}