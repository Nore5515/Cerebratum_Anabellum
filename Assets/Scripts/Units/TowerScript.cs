using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerScript : Unit
{
    public GameObject HPBar;
    public string towerTeam;
    SpawnedUnitStats towerStats;

    Sprite[] rotationSheet;

    // Start is called before the first frame update
    void Start()
    {
        rotationSheet = Resources.LoadAll<Sprite>("Asset_TurretRotationSheet");
        Debug.Log(rotationSheet.Length);
        // Core stat initialization
        hp = 20;
        maxHP = hp;
        dmg = 1;
        speed = 0;
        rof = 0.5f;
        threatLevel = 0;
        unitType = "Tower";
        towerStats = new SpawnedUnitStats();
        towerStats.fireDelay = rof;
        towerStats.unitRange = 3.0f;

        HPBar.GetComponent<Slider>().value = hp;
        HPBar.GetComponent<Slider>().maxValue = maxHP;

        unitObj = this.gameObject;

        Initalize(new List<GameObject>(), towerTeam, towerStats);
    }

    // Update is called once per frame
    void Update()
    {
        MovementUpdate();
        HPBar.GetComponent<Slider>().value = hp;
    }
}
