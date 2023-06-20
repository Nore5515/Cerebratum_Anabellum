using UnityEngine;
using UnityEngine.UI;

public enum BotCommandString
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
    string botCommandTeam;
    BotCommandString botCommandString;

    public BotCommandModel(string botTeam, BotCommandString botCmdString)
    {
        botCommandTeam = botTeam;
        botCommandString = botCmdString;
    }
}