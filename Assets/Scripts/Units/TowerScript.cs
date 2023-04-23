using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerScript : Unit
{
    public GameObject HPBar;

    // Start is called before the first frame update
    void Start()
    {
        // Core stat initialization
        hp = 20;
        maxHP = hp;
        dmg = 1;
        speed = 0;
        rof = 0.5f;
        
        HPBar.GetComponent<Slider>().value = hp;
        HPBar.GetComponent<Slider>().maxValue = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        MovementUpdate();
    }
}
