using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerupButton : MonoBehaviour
{

    public Sprite fireball;
    public Sprite skull;

    [SerializeField]
    Image buttonImage;
    [SerializeField]
    Text buttonText;

    public void SetPowerupButtonType(string powerupType)
    {
        if (powerupType == "Fireball")
        {
            buttonImage.sprite = fireball;
            buttonText.text = "FIREBALL";
        }
        else if (powerupType == "Skull")
        {
            buttonImage.sprite = skull;
            buttonText.text = "SKULL";
        }
        else
        {
            buttonImage.sprite = null;
            buttonText.text = "--";
        }
    }

    public void ButtonPressed()
    {
        // TODO: MAEK SMART LATER
        if (buttonText.text == "FIREBALL")
        {
            Debug.Log("FIREBALLL");
        }
        if (buttonText.text == "SKULL")
        {
            Debug.Log("SKULL");
        }
    }
}
