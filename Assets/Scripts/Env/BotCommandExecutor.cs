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
        switch (commandModel.botCommandString)
        {
            case BotCommands.DoNothing:
                ExecuteDoNothingCommand();
                break;
            case BotCommands.BuildInfSpawner:
                ExecuteBuildInfSpawnerCommand();
                break;
            case BotCommands.ChangeSpawnerPath:
                ExecuteChangeSpawnerPathCommand();
                break;
            default:
                break;
        }
    }

    private void ExecuteChangeSpawnerPathCommand()
    {
        List<BuildingSlot> spawnerSlotList = GetSpawnerBuildingSlots();

        if (spawnerSlotList.Count >= 1)
        {
            int randomSlot = Random.Range(0, spawnerSlotList.Count);
            spawnerSlotList[randomSlot].getSpawner().spawnerPathManager.PickAndCreateNewPath(botTeam);
        }
    }

    private List<BuildingSlot> GetSpawnerBuildingSlots()
    {
        List<BuildingSlot> spawnerSlotList = new List<BuildingSlot>();
        foreach (BuildingSlot buildingSlot in botBuildingSlotList)
        {
            if (buildingSlot.state == "SPAWNER")
            {
                spawnerSlotList.Add(buildingSlot);
            }
        }
        return spawnerSlotList;
    }

    private void ExecuteDoNothingCommand()
    {
        //Debug.Log("Do nothing!");
    }

    private void ExecuteBuildInfSpawnerCommand()
    {
        if (AttemptBuildInfSpawner())
        {
            if (botTeam == "BLUE")
            {
                TeamStats.BlueInfSpawners += 1;
            }
        }
    }

    private bool AttemptBuildInfSpawner()
    {
        if (TeamStats.BluePoints >= Constants.INF_SPAWNER_COST)
        {
            return BuildNewBotInfSpawner();
        }
        return false;
    }

    // TODO: Same idea as in BuildingHandler. Perhaps they can be combined?
    private bool BuildNewBotInfSpawner()
    {
        foreach (BuildingSlot buildingSlot in botBuildingSlotList)
        {
            if (buildingSlot.state == "NONE")
            {
                TeamStats.BluePoints -= Constants.INF_SPAWNER_COST;
                buildingSlot.setState("SPAWNER");
                buildingSlot.infSpawner.GetComponent<Spawner>().LateStart();
                return true;
            }
        }
        return false;
    }
}
