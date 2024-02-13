using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechTree : MonoBehaviour
{
    [SerializeField]
    GameObject techTreeScreen;

    public void SetScreen(GameObject techTreeInstance)
    {
        techTreeScreen = techTreeInstance;
    }

    public void ClickButton()
    {
        Debug.Log("Hello, world!");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            techTreeScreen.SetActive(!techTreeScreen.activeSelf);
        }
    }
}
