using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BestTimes
{
    public static Dictionary<string, float> times = new();

    static bool initialized = false;

    public static void InitializeTimes()
    {
        times.Add("Level1", 0.0f);
        times.Add("Level2", 0.0f);
        times.Add("Level3", 0.0f);
        times.Add("Level4", 0.0f);
        times.Add("Level5", 0.0f);
        initialized = true;
    }

    public static Dictionary<string, float> GetTimes()
    {
        if (!initialized)
        {
            InitializeTimes();
        }
        return times;
    }
}
