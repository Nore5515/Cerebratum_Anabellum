using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpawnerSlider : MonoBehaviour
{
    [SerializeField]
    Slider spawnerSlider;

    [SerializeField]
    TMP_Text spawnerTextCount;

    // Update is called once per frame
    void Update()
    {
        Constants.SPAWNERS_PER_SIDE = (int)spawnerSlider.value;
        spawnerTextCount.text = "Spawners Per Team: " + spawnerSlider.value.ToString();
    }
}
