using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : AdditiveSceneManager
{
	[SerializeField] ButtonAnimator AButton;
    float timeElapsed = 0.0f;
    bool triggered = false;

	void Awake()
	{
		AButton.SetHighlight(true);
	}

    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (!triggered && timeElapsed > 1.0f && SimpleInput.GetInputActive(EInput.A))
        {
            MainManager.Instance.StartGame();
            triggered = true;
        }
    }
}
