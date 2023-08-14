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

    bool settingsMenuOpen = false;


    // TODO: Just change the game object instead o_o
    public GameObject TutorialButton;
    public GameObject StartButton;
    public GameObject ScenarioButton;
    public GameObject SettingsButton;
    public GameObject QuitButton;

    public GameObject Scenarios_EasyMode_Button;
    public GameObject Scenarios_WorldWar1_Button;
    public GameObject Scenarios_NoTower_Button;
    public GameObject Scenarios_Back_Button;

    public GameObject Scenario_Scene;

    public GameObject SettingsGroup;

    public void SetState(string newState)
    {
        HideUI();
        switch (newState)
        {
            case "START":
                SceneManager.LoadScene("Scenes/PlayableScenes/Dual");
                break;
            case "OPEN":
                TutorialButton.SetActive(true);
                StartButton.SetActive(true);
                ScenarioButton.SetActive(true);
                SettingsButton.SetActive(true);
                QuitButton.SetActive(true);
                break;

            case "SCENARIOS":
                Scenario_Scene.SetActive(true);
                break;

            // Load Scenarios
            case "EASYMODE":
                SceneManager.LoadScene("Scenes/Scenarios/EasyMode");
                break;
            case "NOTOWER":
                SceneManager.LoadScene("Scenes/Scenarios/NoTower");
                break;
            case "TUTORIAL":
                SceneManager.LoadScene("Scenes/Scenarios/Tutorial");
                break;
            case "CAMPAIGN":
                SceneManager.LoadScene("Scenes/Campaign/CampaignMenu");
                break;

            // System Stuff
            case "SETTINGS":
                if (settingsMenuOpen)
                {
                    SettingsGroup.SetActive(false);
                    settingsMenuOpen = false;
                }
                else
                {
                    SettingsGroup.SetActive(true);
                    settingsMenuOpen = true;
                }
                TutorialButton.SetActive(true);
                StartButton.SetActive(true);
                ScenarioButton.SetActive(true);
                QuitButton.SetActive(true);
                SettingsButton.SetActive(true);
                break;
            case "QUIT":
                Application.Quit();
                break;

            default:
                Debug.LogError("WRONG STATE SET; MainMenu.cs");
                break;
        }
    }

    public void HideUI()
    {
        TutorialButton.SetActive(false);
        StartButton.SetActive(false);
        ScenarioButton.SetActive(false);
        SettingsButton.SetActive(false);
        QuitButton.SetActive(false);

        Scenarios_EasyMode_Button.SetActive(false);
        Scenarios_WorldWar1_Button.SetActive(false);
        Scenarios_NoTower_Button.SetActive(false);
        Scenarios_Back_Button.SetActive(false);

        SettingsGroup.SetActive(false);
        Scenario_Scene.SetActive(false);
    }
}
