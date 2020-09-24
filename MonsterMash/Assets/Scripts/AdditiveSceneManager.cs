using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SimpleInput))]
public class AdditiveSceneManager : MonoBehaviour
{
    [SerializeField]
    Camera localCamera;
    [SerializeField]
    Canvas localRenderTexture;
    [SerializeField]
    List<string> RequiredScenes = new List<string>();

    void Awake()
    {
        foreach (string s in RequiredScenes)
        {
            if (!SceneManager.GetSceneByName(s).isLoaded)
            {
                SceneManager.LoadScene(s, LoadSceneMode.Additive);
            }
        }

		if (MainManager.Instance != null)
		{
			foreach (Camera c in FindObjectsOfType<Camera>())
            {
                if (c.name == localCamera.name)
                {
                    c.transform.position = localCamera.transform.position;
                    break;
                }
            }
            localCamera.gameObject.SetActive(false);
            localRenderTexture.gameObject.SetActive(false);
		}
    }
}
