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

        // ABSOLUTE LAST THING
        FindObjectOfType<MainManager>().TransOverworldToBattle();
    }
}
