using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleScript : MonoBehaviour
{
    [SerializeField]
    TMP_InputField jsonInput;

    [SerializeField]
    GameObject submitted;

    public void StartGame()
    {
        SceneManager.LoadScene("CampaignMenu", LoadSceneMode.Single);
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

    public void UnitEditor()
    {
        SceneManager.LoadScene("UnitEditor", LoadSceneMode.Single);
    }

    public void SubmitJSON()
    {
        MapJson.Instance.mapJson = jsonInput.text;
        submitted.SetActive(true);
        StartCoroutine(Fadeout());
    }

    IEnumerator Fadeout()
    {
        yield return new WaitForSeconds(2.0f);
        submitted.SetActive(false);
    }
}
