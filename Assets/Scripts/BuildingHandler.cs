using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingHandler : MonoBehaviour
{

    public List<BuildingSlot> buildingSlots = new List<BuildingSlot>();

    public int infSpawnerCost = 1;

    // Start is called before the first frame update
    void Start()
    {
        // foreach (BuildingSlot bs in buildingSlots)
        // {
        //     bs.setState("SPAWNER");
        // }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddInfSpawner()
    {
        foreach (BuildingSlot bs in buildingSlots)
        {
            if (bs.state == "NONE")
            {
                if (TeamStats.RedPoints >= infSpawnerCost)
                {
                    TeamStats.RedPoints -= infSpawnerCost;
                    bs.setState("SPAWNER");
                    bs.infSpawner.GetComponent<Spawner>().LateStart();
                    break;
                }
            }
        }
    }
}
