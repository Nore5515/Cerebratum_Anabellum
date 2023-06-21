using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BotCommandExecutor : MonoBehaviour
{

    [SerializeField]
    private List<BuildingSlot> botBuildingSlotList = new List<BuildingSlot>();
    [SerializeField]
    private string botTeam;

    BotCommandGenerator commandGenerator = new BotCommandGenerator();

    void Start()
    {
        StartCoroutine(BotCommandGenerationTimer());
    }

    IEnumerator BotCommandGenerationTimer()
    {
        yield return new WaitForSeconds(1.0f);
        ExecuteCommand(commandGenerator.GenerateBotCommand(botTeam));
        StartCoroutine(BotCommandGenerationTimer());
    }

    private void ExecuteCommand(BotCommandModel commandModel)
    {
        if (commandModel.botCommandString == BotCommands.DoNothing)
        {
            Debug.Log("Do nothing!");
        }
        else if (commandModel.botCommandString == BotCommands.BuildInfSpawner)
        {
            Debug.Log("Attempt Build Inf Spawner!");
            AttemptBuildInfSpawner();
        }
    }

    private bool AttemptBuildInfSpawner()
    {
        if (TeamStats.BluePoints >= CostConstants.INF_SPAWNER_COST)
        {
            return BuildNewInfSpawner();
        }
        return false;
    }

    // TODO: Same idea as in BuildingHandler. Perhaps they can be combined?
    private bool BuildNewInfSpawner()
    {
        foreach (BuildingSlot buildingSlot in botBuildingSlotList)
        {
            if (buildingSlot.state == "NONE")
            {
                TeamStats.BluePoints -= CostConstants.INF_SPAWNER_COST;
                buildingSlot.setState("SPAWNER");
                buildingSlot.infSpawner.GetComponent<Spawner>().LateStart();
                return true;
            }
        }
        return false;
    }
}
