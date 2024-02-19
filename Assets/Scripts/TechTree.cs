using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TechTree : MonoBehaviour
{
    [SerializeField]
    GameObject techTreeScreen;

    [SerializeField]
    TextMeshProUGUI text_spawnRate;
    [SerializeField]
    TextMeshProUGUI text_range;
    [SerializeField]
    TextMeshProUGUI text_rateOfFire;

    public void SetScreen(GameObject techTreeInstance)
    {
        techTreeScreen = techTreeInstance;
    }

    public void Button_SpawnRate()
    {
        Debug.Log("Hello, world!");
        text_spawnRate.text = "Pressed!";
    }

    public void Button_Range()
    {
        text_range.text = "Pressed!";
    }
    public void Button_RateOfFire()
    {
        text_rateOfFire.text = "Pressed!";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            techTreeScreen.SetActive(!techTreeScreen.activeSelf);
        }
    }
}
