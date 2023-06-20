using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BotCommandGenerator
{

    public BotCommandModel GenerateBotCommand()
    {
        BotCommandModel newCommand = new BotCommandModel("", BotCommands.DoNothing);
        return newCommand;
    }
}
