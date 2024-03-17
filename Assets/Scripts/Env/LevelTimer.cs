using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelTimer : MonoBehaviour
{
    public GameObject TimeTracker;

    public float cycleMax;
    private float cycleVal;

    // Starting Conditions
    public int redNaniteBoost = 0;
    public int blueNaniteBoost = 0;

    [SerializeField]
    int LEVEL_TIME;

    void Start()
    {
        SetCycleMax(LEVEL_TIME);
        cycleVal = cycleMax;

        // Starting Conds
        TeamStats.CycleLength = 15;
        TeamStats.RedNaniteGain = 1;
        TeamStats.BlueNaniteGain = 1;
        TeamStats.RedPoints = redNaniteBoost;
        TeamStats.BluePoints = blueNaniteBoost;

        TimeTracker = GameObject.Find("LevelTimeTracker");
    }

    public void SetCycleMax(float newMax)
    {
        cycleMax = newMax;
    }

    void Update()
    {
        if (cycleVal > 0)
        {
            //Debug.Log("is wurk");
            cycleVal -= 1 * Time.deltaTime;
            TimeTracker.GetComponent<Image>().fillAmount = (cycleVal / cycleMax);
        }
        else
        {
            //TimerEnd();
            Debug.LogError("End Round");
        }

    }

    //public void TimerEnd()
    //{
    //    TeamStats.BlueHP = 0;
    //}


}
