using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapJson
{
    private static MapJson instance = null;
    private static readonly object padlock = new object();

    public string mapJson = "";

    MapJson()
    {
    }

    public static MapJson Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new MapJson();
                }
                return instance;
            }
        }
    }
}