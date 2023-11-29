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
    public Text buttonText;

    public void SetPowerupButtonType(string powerupType)
    {
        if (powerupType == "Fireball")
        {
            buttonImage.sprite = fireball;
            buttonText.text = "FIREBALL";
            this.GetComponent<Button>().interactable = true;
        }
        else if (powerupType == "Skull")
        {
            buttonImage.sprite = skull;
            buttonText.text = "SKULL";
            this.GetComponent<Button>().interactable = true;
        }
        else
        {
            ClearPowerup();
        }
    }

    public void ButtonPressed()
    {
        // TODO: MAEK SMART LATER
        if (buttonText.text == "FIREBALL")
        {
            this.GetComponent<Button>().interactable = !this.GetComponent<Button>().interactable;
            Debug.Log("FIREBALLL");
        }
        if (buttonText.text == "SKULL")
        {
            Debug.Log("SKULL");
        }
    }

    public void ClearPowerup()
    {
        buttonImage.sprite = null;
        buttonText.text = "--";
        this.GetComponent<Button>().interactable = true;
    }

}
