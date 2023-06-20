using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BotCommandGenerator
{

    public BotCommandModel GenerateBotCommand(string team)
    {
        BotCommandModel newCommand;
        // TODO: make more componentized
        if (team == "BLUE")
        {
            if (TeamStats.BluePoints >= CostConstants.INF_SPAWNER_COST)
            {
                if (Random.Range(0.0f, 1.0f) > 0.5f)
                {
                    newCommand = new BotCommandModel("", BotCommands.BuildInfSpawner);
                }
            }
        }
        newCommand = new BotCommandModel("", BotCommands.DoNothing);
        return newCommand;
    }
}
