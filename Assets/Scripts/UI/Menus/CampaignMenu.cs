using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CampaignMenu : MonoBehaviour
{
    public GameObject options;

    public GameObject detailsObj;

    public GameObject levelTitleObj;
    public GameObject levelDescObj;

    private string selectedLevel = "";

    private List<GameObject> levelButtons = new List<GameObject>();

    Dictionary<string, string> levelDetails = new Dictionary<string, string>();
    Dictionary<string, string> levelNames = new Dictionary<string, string>();


    // Start is called before the first frame update
    void Start()
    {
        fillLevelButtons(options);
        designateLevelButtons();
        InitializeLevelFlavor();
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
            levelButton.GetComponent<Button>().onClick.AddListener(delegate { ButtonClick(levelButton); });
        }
    }

    void InitializeLevelFlavor()
    {
        levelDetails.Add("Level1", "Sandsyl is under attack! The Capital is well defended, but without your help it's a losing battle.");
        levelDetails.Add("Level2", "The Irrylians...of course. This next one is tricky, but capture the factories and turn the tide!");
        levelDetails.Add("Level3", "With Sandsyl’s factories back online, they’re eager to get to manufacturing their secret weapon. Some sort of tank that walks on legs, not treads. It’ll take some time to get everything online, and Iryllia is gonna be throwing everything they’ve got to stop those factories. Don’t let them through, and hold the line!");
        levelDetails.Add("Level4", "The Sandsyl people love you, they practically praise your name! Sandsyl is ready for a proper counterattack thanks to your help, and are ready to drive the Iryllians back to their land. With their new tank tech and your leadership, you’ve got a chance! I’ll be signing out after this one, apparently Corporate’s got a new deployment for me. Some Sandsyl agent will be working with you directly from here on out, since I’m leaving. Maybe we’ll be seeing each other, heh!");
        levelNames.Add("Level1", "Rude Awakening");
        levelNames.Add("Level2", "Seizing Power Back");
        levelNames.Add("Level3", "Awaken the War Machine");
        levelNames.Add("Level4", "Returning the Favor");
    }

    void ButtonClick(GameObject buttonObj)
    {
        selectButton(buttonObj);
        UpdateLevelDetails(buttonObj);
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
        Debug.Log(selectedLevel);
    }

    void UpdateLevelDetails(GameObject buttonObj)
    {
        Debug.Log(levelNames.Keys.ToString());
        Debug.Log(buttonObj.name);
        if (levelNames[buttonObj.name] != null && levelDetails[buttonObj.name] != null)
        {
            levelTitleObj.GetComponent<TextMeshProUGUI>().text = levelNames[buttonObj.name];
            levelDescObj.GetComponent<TextMeshProUGUI>().text = levelDetails[buttonObj.name];
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

    LevelData ReadJsonFromFile(string filePath)
    {
        try
        {
            // Read the JSON file as a string
            string jsonContent = File.ReadAllText(filePath);

            // Deserialize the JSON string using JsonUtility
            LevelData jsonData = JsonUtility.FromJson<LevelData>(jsonContent);

            return jsonData;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"An error occurred: {ex.Message}");
            return null;
        }
    }

    void tryParseLevelData()
    {
        string levelJsonFilePath = Path.Combine("..", "..", "JSONData", "levelData.json");

        try
        {

            string jsonData = File.ReadAllText(levelJsonFilePath);

            //LevelData jsonData = JsonUtility.ReadJsonFromFile("data.json");

            //if (jsonData != null)
            //{
            //    Debug.Log($"Name: {jsonData.Name}, Age: {jsonData.Age}");
            //}
            //else
            //{
            //    Debug.LogError("Failed to read JSON data.");
            //}

            // Deserialize the JSON into a dynamic object
            //dynamic data = JsonConvert.DeserializeObject(jsonData);

            // If you have a class for deserialization
            //LevelData data = JsonSerializer.Deserialize<LevelData>(jsonData);

            // Now you can access data.Property1, data.Property2, etc.
            //Console.WriteLine(data.Property1);
            //Console.WriteLine(data.Property2);
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
