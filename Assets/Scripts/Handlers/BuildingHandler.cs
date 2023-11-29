using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingHandler : MonoBehaviour
{

    public List<BuildingSlot> buildingSlots = new List<BuildingSlot>();

    // public void AddInfSpawner()
    // {
    //     foreach (BuildingSlot buildingSlot in buildingSlots)
    //     {
    //         if (buildingSlot.state == "NONE")
    //         {
    //             if (TeamStats.RedPoints >= Constants.INF_SPAWNER_COST)
    //             {
    //                 TeamStats.RedPoints -= Constants.INF_SPAWNER_COST;
    //                 buildingSlot.setState("SPAWNER");
    //                 buildingSlot.infSpawner.GetComponent<Spawner>().LateStart("Infantry");
    //                 break;
    //             }
    //         }
    //     }
    // }

    // public void AddScoutSpawner()
    // {
    //     foreach (BuildingSlot buildingSlot in buildingSlots)
    //     {
    //         if (buildingSlot.state == "NONE")
    //         {
    //             if (TeamStats.RedPoints >= Constants.SCOUT_SPAWNER_COST)
    //             {
    //                 TeamStats.RedPoints -= Constants.SCOUT_SPAWNER_COST;
    //                 buildingSlot.setState("SPAWNER");
    //                 buildingSlot.infSpawner.GetComponent<Spawner>().LateStart("Scout");
    //                 break;
    //             }
    //         }
    //     }
    // }

    // public void AddNaniteGen()
    // {
    //     foreach (BuildingSlot buildingSlot in buildingSlots)
    //     {
    //         if (buildingSlot.state == "NONE")
    //         {
    //             if (TeamStats.RedPoints >= Constants.NANITE_GEN_COST)
    //             {
    //                 TeamStats.RedPoints -= Constants.NANITE_GEN_COST;
    //                 buildingSlot.setState("NANITE");
    //                 break;
    //             }
    //         }
    //     }
    // }

    // public void AddSpidertankSpawner()
    // {
    //     foreach (BuildingSlot buildingSlot in buildingSlots)
    //     {
    //         if (buildingSlot.state == "NONE")
    //         {
    //             if (TeamStats.RedPoints >= Constants.SPIDERTANK_SPAWNER_COST)
    //             {
    //                 TeamStats.RedPoints -= Constants.SPIDERTANK_SPAWNER_COST;
    //                 buildingSlot.setState("SPIDER");
    //                 buildingSlot.spidertankSpawner.GetComponent<Spawner>().LateStart("Spider");
    //                 break;
    //             }
    //         }
    //     }
    // }
}
