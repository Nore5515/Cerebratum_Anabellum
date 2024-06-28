using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelTime : MonoBehaviour
{
    public float time = 0.0f;
    public bool running = true;

    [SerializeField]
    TextMeshProUGUI timerText;

    // Update is called once per frame
    void Update()
    {
        if (running)
        {
            time += Time.deltaTime;
        }
        timerText.text = string.Format("{0:####.##}", time);
    }
}
