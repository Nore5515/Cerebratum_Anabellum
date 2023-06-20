using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BotCommandGenerator : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    public BotCommandModel GenerateBotCommand()
    {
        BotCommandModel newCommand = new BotCommandModel("", BotCommandString.DoNothing);
        return newCommand;
    }
}
