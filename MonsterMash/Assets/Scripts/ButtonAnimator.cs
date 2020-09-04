using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAnimator : MonoBehaviour
{
	[SerializeField] EInput ButtonType;
	[SerializeField] Animator Animator;

	void Update()
	{
		Animator.SetBool("Pressed", SimpleInput.GetInputActive(ButtonType));
	}

	public void SetShow(bool show)
	{
		gameObject.SetActive(show);
	}

	public void SetHighlight(bool highlight)
	{
		Animator.SetBool("Highlight", highlight);
	}
}
