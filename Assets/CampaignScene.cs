using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CampaignScene : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI bestTime;

    [SerializeField]
    TextMeshProUGUI levelTitle;

    string selectedLevel = "";
    // first value is screen level
    // second value is actual level name
    // the reason for this is if we want fun screen level names but in reality
    //  are storing them under a different title
    Dictionary<string, string> visualToActualNamePairs = new();

    private void Start()
    {
        visualToActualNamePairs.Add("Level1", "Level1");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
    }

    public void SelectLevel(string newLevel)
    {
        selectedLevel = newLevel;
        UpdateLevelPreview();
    }

    void UpdateLevelPreview()
    {
        bestTime.text = string.Format("{0:##.000}", GetBestTime());
        levelTitle.text = selectedLevel;
    }

    public void StartLevel()
    {
        if (visualToActualNamePairs.ContainsKey(selectedLevel))
        {
            SceneManager.LoadScene(visualToActualNamePairs[selectedLevel], LoadSceneMode.Single);
        }
        else
        {
            Debug.LogError("ERROR: level " + selectedLevel + " not found!");
        }
    }

    // TODO: Implement
    float GetBestTime()
    {
        // Access best times
        // Load best time by selectedLevel
        return BestTimes.GetTimes()[visualToActualNamePairs[selectedLevel]];
    }
}
