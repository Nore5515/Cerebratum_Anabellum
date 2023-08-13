using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BotCommandGenerator
{
    public BotCommandModel GenerateBotCommand(string team)
    {
        BotCommandModel newCommand = new BotCommandModel("", BotCommands.DoNothing);
        if (team == "BLUE")
        {
            if (TeamStats.BlueInfSpawners < TeamStats.BlueBuildingSlots)
            {
                newCommand = CommandGenWhileEmptySlots();
            }
            else
            {
                newCommand = CommandGenWhileSlotsFull();
            }
        }
        return newCommand;
    }

    private BotCommandModel CommandGenWhileEmptySlots()
    {
        if (TeamStats.BluePoints >= Constants.INF_SPAWNER_COST)
        {
            if (Random.Range(0.0f, 1.0f) < Constants.BOT_CREATE_INF_SPAWNER_CHANCE)
            {
                return new BotCommandModel("", BotCommands.BuildInfSpawner);
            }
        }
        return new BotCommandModel("", BotCommands.DoNothing);
    }

    private BotCommandModel CommandGenWhileSlotsFull()
    {
        if (Random.Range(0.0f, 1.0f) < Constants.BOT_UPGRADE_CHANCE)
        {
            return new BotCommandModel("", BotCommands.UpgradeRandomSpawner);
        }
        if (Random.Range(0.0f, 1.0f) < Constants.BOT_CHANGE_SPAWNER_PATH_CHANCE)
        {
            return new BotCommandModel("", BotCommands.ChangeSpawnerPath);
        }
        return new BotCommandModel("", BotCommands.DoNothing);
    }

}
