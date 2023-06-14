using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSlot : MonoBehaviour
{

    public string state = "SPAWNER";
    public GameObject infSpawner;
    public GameObject naniteGen;
    public GameObject removeButton;
    public string team;

    public bool startingSpawner;

    public void Start()
    {
        if (startingSpawner)
        {
            naniteGen.SetActive(false);
        }
        else
        {
            disableAll();
            state = "NONE";
        }
    }

    public void setState(string newState)
    {
        state = newState;
        processState();
    }

    public void disableAll()
    {
        infSpawner.SetActive(false);
        naniteGen.SetActive(false);
        removeButton.SetActive(false);
    }

    public void processState()
    {
        disableAll();
        switch (state)
        {
            case "SPAWNER":
                infSpawner.SetActive(true);
                removeButton.SetActive(true);
                break;
            case "NANITE":
                naniteGen.SetActive(true);
                removeButton.SetActive(true);
                break;
            case "NONE":
                break;
            default:
                break;
        }
    }
}
