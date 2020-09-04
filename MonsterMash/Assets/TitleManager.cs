using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : AdditiveSceneManager
{
    float timeElapsed = 0.0f;
    bool triggered = false;
    // Update is called once per frame
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
