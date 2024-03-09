using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerScript : Unit
{
    public GameObject HPBar;
    public string towerTeam;
    SpawnedUnitStats towerStats;

    // Start is called before the first frame update
    void Start()
    {
        // Core stat initialization
        unitStats.hp = 20;
        unitStats.maxHP = unitStats.hp;
        unitStats.dmg = 1;
        unitStats.speed = 0;
        unitStats.rof = 0.5f;
        unitStats.threatLevel = 0;
        unitStats.unitType = "Tower";
        towerStats = new SpawnedUnitStats();
        towerStats.fireDelay = unitStats.rof;
        towerStats.unitRange = 3.0f;

        HPBar.GetComponent<Slider>().value = unitStats.hp;
        HPBar.GetComponent<Slider>().maxValue = unitStats.maxHP;

        unitObj = this.gameObject;

        Initalize(new List<Vector3>(), towerTeam, towerStats);
    }

    // Update is called once per frame
    void Update()
    {
        MovementUpdate();
        HPBar.GetComponent<Slider>().value = unitStats.hp;
    }
}
