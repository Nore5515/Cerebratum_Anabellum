using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DaylightHandler : MonoBehaviour
{
    public GameObject daylight;
    public float rotateSpeed;
    public GameObject dayLabel;
    int dayCount;
    bool dayCounted;

    // Start is called before the first frame update
    void Start()
    {
        dayCount = 0;
        dayCounted = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Speed up at nighttime!
        float dayRotate = daylight.transform.rotation.eulerAngles.x;
        if (dayRotate >= 180)
        {
            dayLabel.GetComponent<Text>().text = "NIGHT " + dayCount.ToString();
            daylight.transform.Rotate(new Vector3(rotateSpeed * Time.deltaTime * 10, 0.0f, 0.0f));
            dayCounted = false;
        }
        else{
            daylight.transform.Rotate(new Vector3(rotateSpeed * Time.deltaTime, 0.0f, 0.0f));
            dayLabel.GetComponent<Text>().text = "DAY " + dayCount.ToString();
            if (!dayCounted)
            {
                dayCount += 1;
                dayCounted = true;
            }
        }

    }
}
