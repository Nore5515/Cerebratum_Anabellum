using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Popup : MonoBehaviour
{

    public string title;
    public string commands;
    [TextArea(4, 4)]
    public string desc;
    public GameObject next;
    Button nextButton;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.Find("Panel/PopupTitle").GetComponent<TextMeshProUGUI>().text = title;
        this.transform.Find("Panel/PopupText").GetComponent<TextMeshProUGUI>().text = desc;
        nextButton = this.transform.Find("Panel/NextButton").GetComponent<Button>();
        nextButton.onClick.AddListener(delegate { CycleNextPopup(); });

        switch (commands)
        {
            case "TUT_move":

                break;
            default:
                break;
        }
    }

    void CycleNextPopup()
    {
        if (next != null)
        {
            next.SetActive(true);
        }
        this.gameObject.SetActive(false);
    }
}
