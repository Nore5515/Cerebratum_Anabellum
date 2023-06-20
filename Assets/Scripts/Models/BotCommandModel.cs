using UnityEngine;
using UnityEngine.UI;

public enum BotCommands
{
    BuildInfSpawner,
    RaiseInfSpawn,
    RaiseInfRange,
    RaiseInfRof,
    BuildNaniteGen,
    DoNothing,
}

public class BotCommandModel
{
    public string botCommandTeam;
    public BotCommands botCommandString;

    public BotCommandModel(string botTeam, BotCommands botCmdString)
    {
        botCommandTeam = botTeam;
        botCommandString = botCmdString;
    }
}