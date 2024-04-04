using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.Diagnostics;

public class MyLogHandler : ILogHandler
{
    public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
    {
        Debug.unityLogger.logHandler.LogFormat(logType, context, format, args);
    }

    public void LogException(Exception exception, UnityEngine.Object context)
    {
        Debug.unityLogger.LogException(exception, context);
    }
}

public class EnemyCommander : MonoBehaviour
{
    public List<GameObject> mySpawners = new List<GameObject>();

    public string myTeam = Constants.BLUE_TEAM;
    private static string kTAG = "EnemyCommanderTag";
    private Logger myLogger;


    // Start is called before the first frame update
    void Start()
    {
        myLogger = new Logger(new MyLogHandler());
        PopulateSpawners();
        InvokeRepeating("EnemyLogicTick", 5.0f, 5.0f);
    }

    void PopulateSpawners()
    {
        GameObject[] allSpawners = GameObject.FindGameObjectsWithTag("spawner");
        foreach (GameObject spawner in allSpawners)
        {
            if (spawner.GetComponent<Spawner>() != null)
            {
                if (spawner.GetComponent<Spawner>().spawnerTeam == myTeam)
                {
                    mySpawners.Add(spawner);
                }
            }
        }
    }

    void EnemyLogicTick()
    {
        foreach (GameObject spawner in mySpawners)
        {
            spawner.GetComponent<Spawner>().SpawnScout();
            spawner.GetComponent<Spawner>().spawnerPathManager.AI_DrawPath(transform.position);
            myLogger.Log(kTAG, "Attempting to spawn scout!");
        }
    }
}
