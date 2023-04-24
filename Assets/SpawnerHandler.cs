using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnerHandler : MonoBehaviour
{
    Button spidertankSpawner;

    // Start is called before the first frame update
    void Start()
    {
        InitializeUI();
    }

    void InitializeUI()
    {
        GameObject buildings = GameObject.Find("Canvas/Buildings");
        
        // Buttons
        spidertankSpawner = buildings.transform.Find("AddSpidertankSpawner").gameObject.GetComponent<Button>();
        spidertankSpawner.onClick.AddListener(delegate { AddSpidertankSpawner();});
    }

    void AddSpidertankSpawner()
    {
        Debug.Log("Creating new spawner! Count is : " + SpawnerTracker.redSpawnerObjs.Count);

        if (SpawnerTracker.redSpawnerObjs.Count <= 2)
        {
            if (TeamStats.RedPoints >= 10)
            {
                TeamStats.RedPoints -= 10;
                Debug.Log("New spawner about to be added. Count is : " + SpawnerTracker.redSpawnerObjs.Count);
                Vector3 newPos = SpawnerTracker.redSpawnerObjs[0].transform.position;
                newPos.z += (16.0f * (SpawnerTracker.redSpawnerObjs.Count - 1.5f));
                GameObject newObj = Instantiate(SpawnerTracker.redSpawnerObjs[0], newPos, Quaternion.identity) as GameObject;
                newObj.GetComponent<Structure>().type = "spawn";
                SpawnerTracker.redSpawnerObjs.Add(newObj);
                Debug.Log("New spawner added. Count is : " + SpawnerTracker.redSpawnerObjs.Count);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
