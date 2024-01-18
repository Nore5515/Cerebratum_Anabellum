using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TestButton : MonoBehaviour
{
    public TextMeshProUGUI textmesh;
    bool toggle = false;

    public void ChangeText()
    {
        if (toggle)
        {
            textmesh.text = "Bleh";
        }
        else
        {
            textmesh.text = "Oooh!";
        }

        toggle = !toggle;
    }
}
