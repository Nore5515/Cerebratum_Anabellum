using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSlot : MonoBehaviour
{

    public string state = "SPAWNER";
    public GameObject infSpawner;
    public GameObject spidertankSpawner;
    public GameObject naniteGen;
    public GameObject removeButton;
    public string team;

    public bool startingSpawner;

    public void Start()
    {
        if (startingSpawner)
        {
            naniteGen.SetActive(false);
            spidertankSpawner.SetActive(false);
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
        spidertankSpawner.SetActive(false);
        removeButton.SetActive(false);
    }

    public void processState()
    {
        disableAll();
        switch (state)
        {
            case "SPAWNER":
                ActivateInfSpawner();
                break;
            case "SPIDER":
                ActivateSpidertankSpawner();
                break;
            case "NANITE":
                ActivateNaniteGen();
                break;
            case "NONE":
                ClearBuildingStates();
                break;
            default:
                break;
        }
    }

    private void ActivateSpidertankSpawner()
    {
        spidertankSpawner.SetActive(true);
        removeButton.SetActive(true);
    }

    private void ClearBuildingStates()
    {
        naniteGen.GetComponent<NaniteGen>().StopGen();
    }

    private void ActivateNaniteGen()
    {
        naniteGen.SetActive(true);
        naniteGen.GetComponent<NaniteGen>().InitializeWithTeam(team);
        naniteGen.GetComponent<NaniteGen>().StartGen();
        removeButton.SetActive(true);
    }

    private void ActivateInfSpawner()
    {
        infSpawner.SetActive(true);
        removeButton.SetActive(true);
    }

    public Spawner getSpawner()
    {
        return infSpawner.GetComponent<Spawner>();
    }
}
