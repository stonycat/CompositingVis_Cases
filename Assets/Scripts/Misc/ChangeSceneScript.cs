using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneScript : MonoBehaviour
{
    int x = 0;
    public void ChangeScene()
    {
        if (x == 1)
        {
            Debug.Log("ahahahaa");
            int idx = SceneManager.GetActiveScene().buildIndex;
            idx++;
            if (idx == SceneManager.sceneCountInBuildSettings)
            {
                idx = 0;
            }
            SceneManager.LoadScene(idx);
        } else
        {
            x++;
        }
    }

    public void test()
    {
        Debug.Log("dasdasdasd");
    }
}
