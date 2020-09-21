using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverManager : AdditiveSceneManager
{	

	[SerializeField] Animator GameOverAnimator;

	[SerializeField] AudioClip LostAudioClip;
	[SerializeField] AudioClip WonAudioClip;

    float timeElapsed = 0.0f;
    bool triggered = false;

	public void Setup(bool wonTheGame)
	{
		MMLogger.Log($"GameOver wonTheGame: {wonTheGame}");
		GameOverAnimator.SetBool("Won", wonTheGame);

		var audioClip = wonTheGame ? WonAudioClip : LostAudioClip;
		AudioSource.PlayClipAtPoint(audioClip, transform.position);
	}

    void Update()
    {
        timeElapsed += Time.deltaTime;
        if(!triggered && timeElapsed > 1.0f && SimpleInput.GetInputState(EInput.A) == EButtonState.Released)
        {
            FlowManager.Instance.TransToTitle(Settings.SceneGameOver);
            triggered = true;
        }
    }
}
