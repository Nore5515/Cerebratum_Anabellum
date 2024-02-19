using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Scout : Unit
{
    // Core unit stats
    public float survivalTime = 15.0f;
    public PathHandler cm;
    public Animation anim;
    public bool debugMode = false;

    public void CScout()
    {
        unitObj = gameObject;
        controlDirection = new Vector3(0, 0, 0);
        MIN_DIST_TO_MOVEMENT_DEST = 1;

        // Core stat initialization
        unitStats.hp = Constants.SCOUT_HP;
        unitStats.maxHP = unitStats.hp;
        unitStats.dmg = Constants.SCOUT_DMG;
        unitStats.speed = Constants.SCOUT_SPEED;
        unitStats.rof = Constants.SCOUT_INIT_FIRE_DELAY;
        unitStats.threatLevel = 3;
        unitType = "Scout";

        if (debugMode)
        {
            SpawnedUnitStats debugSpawnedUnitStats = new SpawnedUnitStats();
            debugSpawnedUnitStats.fireDelay = Constants.SCOUT_INIT_FIRE_DELAY;
            debugSpawnedUnitStats.spawnDelay = Constants.SCOUT_INIT_SPAWN_DELAY;
            debugSpawnedUnitStats.unitRange = Constants.SCOUT_INIT_RANGE;
            Initalize(new List<GameObject>(), "RED", debugSpawnedUnitStats);
        }
    }

    public override void SpecializedInitialization()
    {
        CScout();
        unitStats.hp = unitStats.maxHP;
        if (hpSlider != null)
        {
            hpSlider.maxValue = unitStats.maxHP;
            hpSlider.value = unitStats.hp;
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

