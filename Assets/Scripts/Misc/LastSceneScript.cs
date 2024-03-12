using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LastSceneScript : MonoBehaviour
{
    int x = 0;
    public void LastScene()
    {
        if (x == 1)
        {
            int idx = SceneManager.GetActiveScene().buildIndex;
            idx--;
            if (idx == -1)
            {
                idx = SceneManager.sceneCountInBuildSettings -1;
            }
            SceneManager.LoadScene(idx);
        } else
        {
            x++;
        }
    }

    public void test()
    {
        //Debug.Log("dasdasdasd");
    }
}
