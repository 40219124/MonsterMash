using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditiveSceneManager : MonoBehaviour
{
    [SerializeField]
    Camera localCamera;
    [SerializeField]
    Canvas localRenderTexture;

    // Start is called before the first frame update
    void Start()
    {
        if(FindObjectOfType<MainManager>() != null)
        {
            foreach(Camera c in FindObjectsOfType<Camera>())
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
