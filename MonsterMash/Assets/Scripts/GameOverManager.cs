using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverManager : AdditiveSceneManager
{	

	[SerializeField] Animator GameOverAnimator;
    float timeElapsed = 0.0f;
    bool triggered = false;

	public void Setup(bool wonTheGame)
	{
		MMLogger.Log($"GameOver wonTheGame: {wonTheGame}");
		GameOverAnimator.SetBool("Won", wonTheGame);
	}

    void Update()
    {
        timeElapsed += Time.deltaTime;
        if(!triggered && timeElapsed > 1.0f && SimpleInput.GetInputState(EInput.A) == EButtonState.Released)
        {
            FindObjectOfType<FlowManager>().TransToTitle(Settings.SceneGameOver);
            triggered = true;
        }       
    }
}
