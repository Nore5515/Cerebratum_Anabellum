using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CratePrefab : MonoBehaviour
{

    public Slider fillSlider;
    public DetectorSphere detectorSphere;

    // TODO: Better way of linking these
    public PowerupButton powerupButton;

    //string[] powerupList = { "Skull", "Fireball" };
    string[] powerupList = { "Fireball" };

    // Update is called once per frame
    void Update()
    {
        if (detectorSphere.unitInRange)
        {
            fillSlider.value += Time.deltaTime;
            if (fillSlider.value >= fillSlider.maxValue)
            {
                fillSlider.value = 0;
                SendPowerupToButton();
            }
        }
    }

    void SendPowerupToButton()
    {
        powerupButton.SetPowerupButtonType(GenerateRandomPowerup());
    }

    public string GenerateRandomPowerup()
    {
        int randomPowerupIndex = Random.Range(0, powerupList.Length);
        if (powerupList.Length <= 0)
        {
            return "No Powerups Available!";
        }
        return powerupList[randomPowerupIndex];
    }
}
