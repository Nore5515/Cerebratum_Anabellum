using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Text healthText;
    public Text pointsText;
    public Text gameOverText;
    int blueHP = 10;
    int redHP = 10;
    bool gameEnding = false;

    public int bluePoints = 0;
    public int redPoints = 0;

    // Start is called before the first frame update
    void Start()
    {
        healthText.text = "RED: 10             BLUE: 10";
        pointsText.text = "RED: 0               BLUE: 0";
    }

    public void SetNewHealth(int redHP2, int blueHP2)
    {
        healthText.text = "RED: " + redHP2.ToString() + "             BLUE: " + blueHP2.ToString();
    }

    public void ChangePoints(int redChange, int blueChange)
    {
        // Debug.Log("Points Changed!");
        redPoints += redChange;
        bluePoints += blueChange;
        pointsText.text = "RED: " + redPoints.ToString() + "               BLUE: " + bluePoints.ToString();
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
