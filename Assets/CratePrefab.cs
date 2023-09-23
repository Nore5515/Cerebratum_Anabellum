using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CratePrefab : MonoBehaviour
{

    public Slider fillSlider;
    public DetectorSphere detectorSphere;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (detectorSphere.unitInRange)
        {
            fillSlider.value += Time.deltaTime;
            if (fillSlider.value >= fillSlider.maxValue)
            {
                fillSlider.value = 0;
            }
        }
    }
}
