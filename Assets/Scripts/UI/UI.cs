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
    int blueHP = 10;
    int redHP = 10;
    bool gameEnding = false;

    // Start is called before the first frame update
    void Start()
    {
        nanitesText = GameObject.Find("NanitesText").GetComponent<Text>();
        nanitesPerMinuteText = GameObject.Find("NaniteGainText").GetComponent<Text>();
        healthText.text = "RED: 10             BLUE: 10";
        nanitesText.text = "RED: 0               BLUE: 0";
        nanitesPerMinuteText.text = "RED: 0               BLUE: 0";
    }

    public void SetNewHealth(int redHP2, int blueHP2)
    {
        healthText.text = "RED: " + redHP2.ToString() + "             BLUE: " + blueHP2.ToString();
    }

    void Update() 
    {
        nanitesText.text = "RED: " + TeamStats.RedPoints.ToString() + "               BLUE: " + TeamStats.BluePoints.ToString();
        nanitesPerMinuteText.text = "RED: " + TeamStats.RedNaniteGain.ToString() + "               BLUE: " + TeamStats.BlueNaniteGain.ToString();
    }

    public void DecrementHealth(GameObject obj)
    {
        if (obj.name == "BlueSpawner")
        {
            blueHP -= 1;
        }
        else
        {
            redHP -= 1;
        }
        if (blueHP <= 0 || redHP <= 0)
        {
            if (!gameEnding)
            {
                gameEnding = true;
                if (blueHP > redHP)
                {
                    gameOverText.text = "Blue Victory!";
                }
                else
                {
                    gameOverText.text = "Red Victory!";
                }
                IEnumerator coroutine = EndGame();
                StartCoroutine(coroutine);
            }
        }
        SetNewHealth(redHP, blueHP);
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(5.0f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
