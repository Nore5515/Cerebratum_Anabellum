using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spidertank : Unit
{
    // Core unit stats
    public float survivalTime = 15.0f;
    public StompSphere stomp;

    public void CSpidertank()
    {
        unitObj = this.gameObject;
        controlDirection = new Vector3(0, 0, 0);
        MaxDist = 1.4;
        MinDist = 1;

        // Core stat initialization
        hp = 20;
        maxHP = hp;
        dmg = 1;
        speed = 2;
        rof = 2f;
        threatLevel = 20;
    }

    void Start()
    {
        CSpidertank();
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
        // TODO: Make this better?
        if (stomp.alliedTeam != team)
        {
            stomp.alliedTeam = team;
        }
        MovementUpdate();
    }
}

