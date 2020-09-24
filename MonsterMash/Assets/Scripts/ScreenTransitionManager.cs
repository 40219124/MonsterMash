using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTransitionManager : AdditiveSceneManager
{
	[SerializeField] Animator TransitionAnimator;
	public static ScreenTransitionManager Instance;

	bool ShowingBlack;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			MMLogger.LogError("ScreenTransitionManager.Instance already setup");
		}
	}

	public static void SetShowBlack(bool showBlack)
	{
		Instance.TransitionAnimator.SetBool("ShowBlack", showBlack);
		MMLogger.Log($"setting show black to: {showBlack} was {Instance.ShowingBlack}");
		Instance.ShowingBlack = showBlack;
	}

	// void Update()
	// {
	// 	if (SimpleInput.GetInputState(EInput.A) == EButtonState.Released)
	// 	{
	// 		SetShowBlack(!ShowingBlack);
	// 	}

	// }

	public static IEnumerator WaitForSetBlack(bool showBlack)
	{
		SetShowBlack(showBlack);

		AnimatorStateInfo stateInfo;
		do
		{
			yield return null;
			stateInfo = Instance.TransitionAnimator.GetCurrentAnimatorStateInfo(0);
		} while (stateInfo.IsName("To White") || stateInfo.IsName("To Black"));

		yield break;
	}
}
