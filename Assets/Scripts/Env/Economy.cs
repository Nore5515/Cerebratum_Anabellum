using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Economy : MonoBehaviour
{

    public GameObject TimeTracker;

    public float cycleMax;
    private float cycleVal;

    // Starting Conditions
    public int redNaniteBoost = 0;
    public int blueNaniteBoost = 0;

    public GameObject fuckoff;

    // Start is called before the first frame update
    void Start()
    {
        SetCycleMax(15);
        cycleVal = 0;
        
        // TODO: make better cyclemax parse too lasy
        // Starting Conds
        TeamStats.CycleLength = 15;
        TeamStats.RedNaniteGain = 1;
        TeamStats.BlueNaniteGain = 1;
        TeamStats.RedPoints = redNaniteBoost;
        TeamStats.BluePoints = blueNaniteBoost;

        TimeTracker = GameObject.Find("TimeTracker");
    }

    public void SetCycleMax(float newMax)
    {
        cycleMax = newMax;
    }

    void Update()
    {
        // TODO: One second of Flashing noti!
        if (cycleVal < cycleMax)
        {
            cycleVal += 1 * Time.deltaTime;
            TimeTracker.GetComponent<Image>().fillAmount = (cycleVal / cycleMax);
        }
        else
        {
            GainPoints();   
            cycleVal = 0;
        }

        fuckoff.GetComponent<Text>().text = SpawnerTracker.redSpawnerObjs.Count.ToString();
    }

    private void GainPoints()
    {
        
        TeamStats.RedPoints += TeamStats.RedNaniteGain;
        TeamStats.BluePoints += TeamStats.BlueNaniteGain;

        // if (isAI)
        // {
        //     AI_SpendPoints();
        // }
    }
}
