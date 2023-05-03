using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TODO IM ADDING BOTH ENEMY AND ALLY TO THE SAME ONE OH MY GOD
public static class SpawnerTracker
{

    public static List<GameObject> alliedSpawnerObjs = new List<GameObject>();

    //TODO
    public static List<GameObject> redSpawnerObjs = new List<GameObject>();
    public static List<GameObject> blueSpawnerObjs = new List<GameObject>();

    // ROOT SPAWNERS
    public static GameObject redRootSpawner;
    public static GameObject blueRootSpawner;

    public static void NewGame()
    {
        alliedSpawnerObjs = new List<GameObject>();
        redSpawnerObjs = new List<GameObject>();
        blueSpawnerObjs = new List<GameObject>();
        redRootSpawner = null;
        blueRootSpawner = null;
    }

    // // ADD / REMOVE
    // public static void AddSpawner(GameObject newSpawn)
    // {
    //     alliedSpawnerObjs.Add(newSpawn);
    // }
    // public static void RemoveSpawner(GameObject removingSpawn)
    // {
    //     alliedSpawnerObjs.Remove(removingSpawn);
    // }

    // // GET COUNT
    // public static int GetCount()
    // {
    //     return alliedSpawnerObjs.Count;
    // }

    // // GET
    // public static GameObject GetSpawner(int index)
    // {
    //     return alliedSpawnerObjs[index];
    // }

}
