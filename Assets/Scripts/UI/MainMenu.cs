using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour
{


    // STATES
    // Open: [START, SCENARIOS, SETTINGS, QUIT]
    // Scenarios: [EasyMode, WorldWar1, NoTower, Back]

    // Start button -> 1v1
    // Settings -> Settings menu


    // TODO: Just change the game object instead o_o
    public GameObject StartButton;
    public GameObject ScenarioButton;
    public GameObject SettingsButton;
    public GameObject QuitButton;

    public GameObject Scenarios_EasyMode_Button;
    public GameObject Scenarios_WorldWar1_Button;
    public GameObject Scenarios_NoTower_Button;
    public GameObject Scenarios_Back_Button;

    public void SetState(string newState)
    {
        HideUI();
        switch(newState) 
        {
            case "START":
                SceneManager.LoadScene("Scenes/TestScenes/Map_Test");
                break;
            case "OPEN":
                StartButton.SetActive(true);
                ScenarioButton.SetActive(true);
                SettingsButton.SetActive(true);
                QuitButton.SetActive(true);
                break;

            case "SCENARIOS":
                Scenarios_EasyMode_Button.SetActive(true);
                Scenarios_WorldWar1_Button.SetActive(true);
                Scenarios_NoTower_Button.SetActive(true);
                Scenarios_Back_Button.SetActive(true);
                break;

            default:
                Debug.LogError("WRONG STATE SET; MainMenu.cs");
                break;
        }
    }

    public void HideUI()
    {
        StartButton.SetActive(false);
        ScenarioButton.SetActive(false);
        SettingsButton.SetActive(false);
        QuitButton.SetActive(false);

        Scenarios_EasyMode_Button.SetActive(false);
        Scenarios_WorldWar1_Button.SetActive(false);
        Scenarios_NoTower_Button.SetActive(false);
        Scenarios_Back_Button.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
