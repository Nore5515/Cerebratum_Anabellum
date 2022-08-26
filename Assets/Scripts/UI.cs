using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Text healthText;
    int blueHP = 10;
    int redHP = 10;

    // Start is called before the first frame update
    void Start()
    {
        healthText.text = "RED: 10             BLUE: 10";
    }

    public void SetNewHealth(int redHP2, int blueHP2)
    {
        healthText.text = "RED: " + redHP2.ToString() + "             BLUE: " + blueHP2.ToString();
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
        SetNewHealth(redHP, blueHP);
    }
}
