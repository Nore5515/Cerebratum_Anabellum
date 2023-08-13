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
    ChangeSpawnerPath,
    UpgradeRandomSpawner
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