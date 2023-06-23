using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnerHandler : MonoBehaviour
{
    Button spidertankSpawner;
    Button infSpawner;
    public GameObject spidertankSpawnerPrefab;
    public GameObject infSpawnerPrefab;

    int stSpawnerCost = 10;
    // int infSpawnerCost = 1;

    // Start is called before the first frame update
    void Start()
    {
        InitializeUI();
    }

    void InitializeUI()
    {
        GameObject buildings = GameObject.Find("Canvas/Constructions/Buildings");

        // Buttons
        spidertankSpawner = buildings.transform.Find("AddSpidertankSpawner").gameObject.GetComponent<Button>();
        spidertankSpawner.onClick.AddListener(delegate { AddSpidertankSpawner(); });
        // spidertankSpawner = buildings.transform.Find("RAddSpawner").gameObject.GetComponent<Button>();
        // spidertankSpawner.onClick.AddListener(delegate { AddInfSpawner(); });
    }

    // void AddInfSpawner()
    // {
    //     Debug.Log("Creating new spawner! Count is : " + SpawnerTracker.redSpawnerObjs.Count);

    //     if (SpawnerTracker.redSpawnerObjs.Count <= 2)
    //     {
    //         if (TeamStats.RedPoints >= infSpawnerCost)
    //         {
    //             TeamStats.RedPoints -= infSpawnerCost;
    //             Debug.Log("New spawner about to be added. Count is : " + SpawnerTracker.redSpawnerObjs.Count);
    //             Vector3 newPos = SpawnerTracker.redSpawnerObjs[0].transform.position;
    //             newPos.z += (16.0f * (SpawnerTracker.redSpawnerObjs.Count - 1.5f));
    //             GameObject newObj = Instantiate(infSpawnerPrefab, newPos, Quaternion.identity) as GameObject;
    //             newObj.GetComponent<Structure>().type = "spawn";
    //             SpawnerTracker.redSpawnerObjs.Add(newObj);
    //             Debug.Log("New spawner added. Count is : " + SpawnerTracker.redSpawnerObjs.Count);
    //         }
    //     }
    // }

    void AddSpidertankSpawner()
    {
        if (SpawnerTracker.redSpawnerObjs.Count <= 2)
        {
            if (TeamStats.RedPoints >= stSpawnerCost)
            {
                TeamStats.RedPoints -= stSpawnerCost;
                Vector3 newPos = SpawnerTracker.redSpawnerObjs[0].transform.position;
                newPos.z += (16.0f * (SpawnerTracker.redSpawnerObjs.Count - 1.5f));
                GameObject newObj = Instantiate(spidertankSpawnerPrefab, newPos, Quaternion.identity) as GameObject;
                newObj.GetComponent<Structure>().type = "spawn";
                SpawnerTracker.redSpawnerObjs.Add(newObj);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

}
