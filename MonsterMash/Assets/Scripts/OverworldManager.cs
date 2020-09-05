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

	void Awake()
	{
		BButton.SetHighlight(false);
		AButton.SetHighlight(false);
	}

	void Update()
	{
		if (SimpleInput.GetInputState(EInput.Select) == EButtonState.Pressed && !Transitioning)
		{
            DoTransitionToPicker();
		}
	}

    public void DoTransitionToBattle()
    {
        if (!Transitioning)
        {
            Transitioning = true;
            StartCoroutine(DoTransToBattleCo());
        }
    }

    private IEnumerator DoTransToBattleCo()
    {
        yield return WaitForActors();
        // ABSOLUTE LAST THING
        FindObjectOfType<FlowManager>().TransOverworldToBattle();
    }

    public void DoTransitionToPicker()
    {
        if (!Transitioning)
        {
            Transitioning = true;
            StartCoroutine(DoTransToPickerCo());
        }
    }

    private IEnumerator DoTransToPickerCo()
    {
        yield return WaitForActors();
        // ABSOLUTE LAST THING
        FindObjectOfType<FlowManager>().TransToPicker(Settings.SceneOverworld);
    }


    private IEnumerator WaitForActors()
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
