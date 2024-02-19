using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoMenu : MonoBehaviour
{

    public void OnevOne()
    {
        SceneManager.LoadScene("AI_Duel_Scene");
    }

    public void OnevThree()
    {
        SceneManager.LoadScene("AIScene");
    }

    public void NoAI()
    {
        SceneManager.LoadScene("SimpleGameScene");
    }
}
