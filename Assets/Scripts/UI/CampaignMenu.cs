using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CampaignMenu : MonoBehaviour
{
    public GameObject options;

    public GameObject detailsObj;

    private string selectedLevel = "";

    private List<GameObject> levelButtons = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Options " + options);
        fillLevelButtons(options);
        designateLevelButtons();
    }

    void fillLevelButtons(GameObject parentObject)
    {
        foreach (Transform child in parentObject.transform)
        {
            if (child.gameObject.name.Contains("Level"))
            {
                levelButtons.Add(child.gameObject);
            }
        }
    }

    void designateLevelButtons()
    {
        foreach (GameObject levelButton in levelButtons)
        {
            levelButton.GetComponent<Button>().onClick.AddListener(delegate { selectButton(levelButton); });
        }
    }

    void selectButton(GameObject buttonObj)
    {
        if (selectedLevel == buttonObj.name)
        {
            selectedLevel = "";
        }
        else
        {
            selectedLevel = buttonObj.name;
        }
    }

    void loadLevel(string levelName)
    {
        string campaignLevelPath = "Scenes/Campaign/" + levelName;
        SceneManager.LoadScene(campaignLevelPath);
    }

    public void beginLevelPressed()
    {
        if (selectedLevel == "")
        {
            return;
        }
        else
        {
            loadLevel(selectedLevel);
        }
    }

    void tryParseLevelData()
    {
        string levelJsonFilePath = Path.Combine("..", "..", "JSONData", "levelData.json");

        try
        {

            string jsonData = File.ReadAllText(levelJsonFilePath);

            // Deserialize the JSON into a dynamic object
            dynamic data = JsonConvert.DeserializeObject(jsonData);

            // If you have a class for deserialization
            //LevelData data = JsonSerializer.Deserialize<LevelData>(jsonData);

            // Now you can access data.Property1, data.Property2, etc.
            Console.WriteLine(data.Property1);
            Console.WriteLine(data.Property2);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
