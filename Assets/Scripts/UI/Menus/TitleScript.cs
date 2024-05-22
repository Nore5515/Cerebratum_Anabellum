using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleScript : MonoBehaviour
{
    [SerializeField]
    TMP_InputField jsonInput;

    public void StartGame()
    {
        SceneManager.LoadScene("UltimateTest", LoadSceneMode.Single);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void MapTesting()
    {
        SceneManager.LoadScene("CustomMap", LoadSceneMode.Single);
    }

    public void MapEditor()
    {
        SceneManager.LoadScene("MapEditor", LoadSceneMode.Single);
    }

    public void SubmitJSON()
    {
        MapJson.Instance.mapJson = jsonInput.text;
    }
}
