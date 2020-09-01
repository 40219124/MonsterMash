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
		Show(false, false);
	}

	public void Show(bool show, bool selected, bool forceSimple=false)
	{
		StatBoxRoot.SetActive(show);
		StatAnimator.SetBool("Complex", selected && !forceSimple);
		StatAnimator.SetBool("Selected", selected);
	}
	public void SetHealthNumber(bool show, int value=69)
	{
		HealthIconGameObject.SetActive(show);
		HealthNumber.gameObject.SetActive(show);
		HealthNumber.SetNumber(value);
	}
	public void SetDamageNumber(bool show, int value=69)
	{
		DamageIconGameObject.SetActive(show);
		DamageNumber.gameObject.SetActive(show);
		DamageNumber.SetNumber(value);
	}
	public void SetArmourNumber(bool show, int value=69)
	{
		ArmourIconGameObject.SetActive(show);
		ArmourNumber.gameObject.SetActive(show);
		ArmourNumber.SetNumber(value);
	}
	public void SetTimeNumber(bool show, int value=69)
	{
		TimeIconGameObject.SetActive(show);
		TimeNumber.gameObject.SetActive(show);
		TimeNumber.SetNumber(value);
	}
}