using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BotCommandExecutor : MonoBehaviour
{

    BotCommandGenerator commandGenerator = new BotCommandGenerator();

    void Start()
    {
        StartCoroutine(BotCommandGenerationTimer());
    }

    IEnumerator BotCommandGenerationTimer()
    {
        yield return new WaitForSeconds(1.0f);
        ExecuteCommand(commandGenerator.GenerateBotCommand());
        StartCoroutine(BotCommandGenerationTimer());
    }

    public void ExecuteCommand(BotCommandModel commandModel)
    {
        if (commandModel.botCommandString == BotCommands.DoNothing)
        {
            Debug.Log("Do nothing!");
        }
    }

}
