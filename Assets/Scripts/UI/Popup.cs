using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{

    public string title;
    public string desc;
    public GameObject next;
    Button nextButton;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.Find("Panel/PopupTitle").GetComponent<Text>().text = title;
        this.transform.Find("Panel/PopupText").GetComponent<Text>().text = desc;
        nextButton = this.transform.Find("Panel/NextButton").GetComponent<Button>();
        nextButton.onClick.AddListener(delegate { CycleNextPopup(); });
    }

    void CycleNextPopup()
    {
        next.SetActive(true);
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
