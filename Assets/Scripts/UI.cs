using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Text healthText;

    // Start is called before the first frame update
    void Start()
    {
        healthText.text = "RED: 10             BLUE: 10";
    }

    public void SetNewHealth(int redHP, int blueHP)
    {
        healthText.text = "RED: " + redHP.ToString() + "             BLUE: " + blueHP.ToString();
    }
}
