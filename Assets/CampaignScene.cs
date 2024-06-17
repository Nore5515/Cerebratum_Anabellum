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
    Dictionary<string, string> visualToActualNamePairs = new();

    public void SelectLevel(string newLevel)
    {
        selectedLevel = newLevel;
        UpdateLevelPreview();
    }

    void UpdateLevelPreview()
    {
        bestTime.text = GetBestTime().ToString();
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
        return 0.0f;
    }
}
