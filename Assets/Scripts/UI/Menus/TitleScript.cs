using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScript : MonoBehaviour
{

    public void StartGame()
    {
        SceneManager.LoadScene("UltimateTest", LoadSceneMode.Single);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
