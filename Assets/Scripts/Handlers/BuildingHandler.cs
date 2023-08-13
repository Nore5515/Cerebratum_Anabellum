using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingHandler : MonoBehaviour
{

    public List<BuildingSlot> buildingSlots = new List<BuildingSlot>();

    public int infSpawnerCost = 1;
    public int NANITE_GEN_COST = 1;
    public int SPIDER_COST = 1;

    public void AddInfSpawner()
    {
        foreach (BuildingSlot buildingSlot in buildingSlots)
        {
            if (buildingSlot.state == "NONE")
            {
                if (TeamStats.RedPoints >= infSpawnerCost)
                {
                    TeamStats.RedPoints -= infSpawnerCost;
                    buildingSlot.setState("SPAWNER");
                    buildingSlot.infSpawner.GetComponent<Spawner>().LateStart();
                    break;
                }
            }
        }
    }

    public void AddNaniteGen()
    {
        foreach (BuildingSlot buildingSlot in buildingSlots)
        {
            if (buildingSlot.state == "NONE")
            {
                if (TeamStats.RedPoints >= NANITE_GEN_COST)
                {
                    TeamStats.RedPoints -= NANITE_GEN_COST;
                    buildingSlot.setState("NANITE");
                    break;
                }
            }
        }
    }

    public void AddSpidertankSpawner()
    {
        foreach (BuildingSlot buildingSlot in buildingSlots)
        {
            if (buildingSlot.state == "NONE")
            {
                if (TeamStats.RedPoints >= SPIDER_COST)
                {
                    TeamStats.RedPoints -= SPIDER_COST;
                    buildingSlot.setState("SPIDER");
                    buildingSlot.spidertankSpawner.GetComponent<Spawner>().LateStart();
                    break;
                }
            }
        }
    }
}
