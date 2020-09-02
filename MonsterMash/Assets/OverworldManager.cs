using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldManager : AdditiveSceneManager
{
    public delegate void TransitionToBattle();
    public TransitionToBattle OnTransition;

    public void DoTransitionToBattle()
    {
        OnTransition?.Invoke();

        // ABSOLUTE LAST THING
        FindObjectOfType<MainManager>().TransOverworldToBattle();
    }
}
