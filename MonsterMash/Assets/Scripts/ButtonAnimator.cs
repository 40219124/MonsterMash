using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAnimator : MonoBehaviour
{
	[SerializeField] EInput ButtonType;
	[SerializeField] Animator Animator;
	
	[SerializeField] AudioClip HighLightAudioClip;
	[SerializeField] AudioClip PressAudioClip;

	void Update()
	{
		var state = SimpleInput.GetInputState(ButtonType);
		if (state == EButtonState.Pressed)
		{
			AudioSource.PlayClipAtPoint(PressAudioClip, transform.position);
		}

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

	public void EventPlayHighlightAudio()
	{
		AudioSource.PlayClipAtPoint(HighLightAudioClip, transform.position, volume:0.5f);
	}
}
