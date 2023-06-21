using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BotCommandGenerator
{
    public BotCommandModel GenerateBotCommand(string team)
    {
        BotCommandModel newCommand = new BotCommandModel("", BotCommands.DoNothing);
        // TODO: make more componentized
        if (team == "BLUE")
        {
            if (TeamStats.BlueInfSpawners < TeamStats.BlueBuildingSlots)
            {
                if (TeamStats.BluePoints >= CostConstants.INF_SPAWNER_COST)
                {
                    if (Random.Range(0.0f, 1.0f) < CostConstants.BOT_CREATE_INF_SPAWNER_CHANCE)
                    {
                        newCommand = new BotCommandModel("", BotCommands.BuildInfSpawner);
                    }
                }
            }
            else
            {
                if (Random.Range(0.0f, 1.0f) < CostConstants.BOT_CHANGE_SPAWNER_PATH_CHANCE)
                {
                    newCommand = new BotCommandModel("", BotCommands.ChangeSpawnerPath);
                }
            }
        }
        return newCommand;
    }
}
