using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartScene());
    }

    IEnumerator StartScene()
    {
        AsyncOperation aync = SceneManager.LoadSceneAsync("LoadScene2", LoadSceneMode.Additive);
        while (aync.isDone == false)
        {
            Debug.Log(aync.progress);
            yield return null;
        }
        Debug.Log("LoadOK");
        yield return null;
    }
    
}
