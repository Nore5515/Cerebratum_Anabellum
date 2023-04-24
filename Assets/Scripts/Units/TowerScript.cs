using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerScript : Unit
{
    public GameObject HPBar;
    public string towerTeam;

    // Start is called before the first frame update
    void Start()
    {
        // Core stat initialization
        hp = 20;
        maxHP = hp;
        dmg = 1;
        speed = 0;
        rof = 0.5f;
        threatLevel = 0;
        
        HPBar.GetComponent<Slider>().value = hp;
        HPBar.GetComponent<Slider>().maxValue = maxHP;

        unitObj = this.gameObject;

        Initalize(new List<GameObject>(), towerTeam, rof, 3);
    }

    // Update is called once per frame
    void Update()
    {
        MovementUpdate();
    }
}
