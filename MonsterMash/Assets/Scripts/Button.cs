using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button : MonoBehaviour
{
	[SerializeField] GameObject Root;
	[SerializeField] Animator ButtonAnimator;
	[SerializeField] Text ButtonLabel;

	public void SetSelected(bool selected)
	{
		ButtonAnimator.SetBool("Selected", selected);
	}

	public void SetShow(bool show)
	{
		Root.SetActive(show);
	}
}
