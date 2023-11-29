using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Text healthText;
    public Text nanitesText;
    public Text nanitesPerMinuteText;
    public Text gameOverText;
    public Image gameoverBGImage;
    bool gameEnding = false;

    int startingRedHP;

    // Start is called before the first frame update
    void Start()
    {
        startingRedHP = TeamStats.RedHP;
        nanitesText = GameObject.Find("NanitesText").GetComponent<Text>();
        //nanitesPerMinuteText = GameObject.Find("NaniteGainText").GetComponent<Text>();
        healthText.text = "RED: 10 --- BLUE: 10";
        nanitesText.text = "RED: 0 --- BLUE: 0";
        //nanitesPerMinuteText.text = "RED: 0 --- BLUE: 0";
    }

    double GetRedHPPercentage()
    {
        double result = TeamStats.RedHP / startingRedHP;
        result = result * 100;
        return result;
    }

    void Update()
    {

        healthText.text = GetRedHPPercentage().ToString() + "%";
        //healthText.text = "RED: " + TeamStats.RedHP.ToString() + " --- BLUE: " + TeamStats.BlueHP.ToString();
        nanitesText.text = TeamStats.RedPoints.ToString();
        //nanitesText.text = "RED: " + TeamStats.RedPoints.ToString() + " --- BLUE: " + TeamStats.BluePoints.ToString();
        //nanitesPerMinuteText.text = "RED: " + TeamStats.RedNaniteGain.ToString() + " --- BLUE: " + TeamStats.BlueNaniteGain.ToString();

        // if (TeamStats.GameStarted)
        // {
        if (TeamStats.BlueHP <= 0 || TeamStats.RedHP <= 0)
        {
            if (!gameEnding)
            {
                gameEnding = true;
                if (TeamStats.BlueHP > TeamStats.RedHP)
                {
                    gameOverText.text = "Blue Victory!";
                }
                else
                {
                    gameOverText.text = "Red Victory!";
                }
                gameoverBGImage.enabled = true;
                IEnumerator coroutine = EndGame();
                StartCoroutine(coroutine);
            }
        }
        // }
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(5.0f);
        //SpawnerTracker.NewGame();
        TeamStats.ResetGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
