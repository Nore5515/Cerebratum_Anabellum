using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Infantry : Unit
{
    // Core unit stats
    public float survivalTime = 15.0f;
    public CubeMaker cm;
    public Animation anim;

    public void CInfantry()
    {
        unitObj = this.gameObject;
        controlDirection = new Vector3(0, 0, 0);
        MIN_DIST_TO_MOVEMENT_DEST = 1;

        // Core stat initialization
        hp = 1;
        maxHP = hp;
        dmg = 1;
        speed = 4;
        rof = 2f;
        threatLevel = 3;
        unitType = "Infantry";
    }

    void Start()
    {
        CInfantry();
        hp = maxHP;
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHP;
            hpSlider.value = hp;
        }
        if (survivalTime > 0)
        {
            IEnumerator coroutine = SelfDestruct();
            StartCoroutine(coroutine);
        }
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(survivalTime);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        MovementUpdate();
    }
}

