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

    string[] powerupList = { "Skull", "Fireball" };

    // Start is called before the first frame update
    void Start()
    {
        
    }

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


    int stupid = 0;
    public string GenerateRandomPowerup()
    {
        int randomPowerupIndex = Random.Range(0, powerupList.Length);
        randomPowerupIndex = stupid;
        stupid++;
        if (stupid == powerupList.Length)
        {
            stupid = 0;
        }
        if (powerupList.Length <= 0)
        {
            return "No Powerups Available!";
        }
        return powerupList[randomPowerupIndex];
    }
}
