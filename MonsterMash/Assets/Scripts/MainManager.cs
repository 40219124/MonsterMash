using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadOverworld());
    }

    IEnumerator LoadOverworld()
    {
        yield return new WaitForSeconds(2.0f);
        string name = "Tim";
        SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
