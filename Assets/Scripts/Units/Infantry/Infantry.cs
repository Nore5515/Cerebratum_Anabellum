using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Infantry : Unit
{
    // Core unit stats
    public float survivalTime = 15.0f;
    public PathHandler cm;
    public Animation anim;
    public bool debugMode = false;

    public void CInfantry()
    {
        unitObj = gameObject;
        controlDirection = new Vector3(0, 0, 0);
        Constants.MIN_DIST_TO_MOVEMENT_DEST = 1;

        // Core stat initialization
        unitStats.hp = Constants.INF_HP;
        unitStats.maxHP = unitStats.hp;
        unitStats.dmg = Constants.INF_DMG;
        unitStats.speed = Constants.INF_SPEED;
        unitStats.rof = Constants.INF_INIT_FIRE_DELAY;
        unitStats.threatLevel = 3;
        unitStats.unitType = Constants.INF_TYPE;

        if (debugMode)
        {
            SpawnedUnitStats debugSpawnedUnitStats = new SpawnedUnitStats();
            debugSpawnedUnitStats.fireDelay = Constants.INF_INIT_FIRE_DELAY;
            debugSpawnedUnitStats.spawnDelay = Constants.INF_INIT_SPAWN_DELAY;
            debugSpawnedUnitStats.unitRange = Constants.INF_INIT_RANGE;
            Initalize(new List<Vector3>(), "RED", debugSpawnedUnitStats);
        }
    }

    void Start()
    {
        CInfantry();
        unitStats.hp = unitStats.maxHP;
        if (hpSlider != null)
        {
            hpSlider.maxValue = unitStats.maxHP;
            hpSlider.value = unitStats.hp;
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
        IdleUpdate();
    }
}

