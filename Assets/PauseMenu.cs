using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    GameObject visibleParent;

    private void Start()
    {
        visibleParent = transform.GetChild(0).gameObject;
        visibleParent.SetActive(false);
    }

    public void ToggleActive()
    {
        visibleParent.SetActive(!visibleParent.activeSelf);
    }

    public void Resume()
    {
        visibleParent.SetActive(false);
    }

    public void Restart()
    {
        SceneManager.LoadScene("UltimateTest", LoadSceneMode.Single);
    }

    public void BackToMain()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
