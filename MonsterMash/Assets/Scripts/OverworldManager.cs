using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OverworldManager : AdditiveSceneManager
{
	public delegate bool TransitionToBattle();
	public TransitionToBattle OnTransition = null;

	public bool Transitioning = false;

	[SerializeField] ButtonAnimator BButton;
	[SerializeField] ButtonAnimator AButton;

	protected override void Awake()
	{
		base.Awake();
		BButton.SetHighlight(false);
		AButton.SetHighlight(false);
	}

	public void DoTransitionToBattle()
	{
		if (!Transitioning)
		{
			Transitioning = true;
			StartCoroutine(DoTransToBattleCo());
		}
	}

	IEnumerator DoTransToBattleCo()
	{
		yield return WaitForActors();
		// ABSOLUTE LAST THING
		FlowManager.Instance.TransOverworldToBattle();
	}

	public void DoTransitionToPicker()
	{
		if (!Transitioning)
		{
			Transitioning = true;
			StartCoroutine(DoTransToPickerCo());
		}
	}

	IEnumerator DoTransToPickerCo()
	{
		yield return WaitForActors();
		// ABSOLUTE LAST THING
		FlowManager.Instance.TransToPicker(Settings.SceneOverworld);
	}


	IEnumerator WaitForActors()
	{
		bool notReady = true;
		while (notReady)
		{
			notReady = false;
			foreach (TransitionToBattle d in OnTransition.GetInvocationList())
			{
				notReady |= !d.Invoke();
			}
			yield return null;
		}
	}
}
