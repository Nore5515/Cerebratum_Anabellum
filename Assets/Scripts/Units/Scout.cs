using System.Collections;
using System.Collections.Generic;
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
        hp = Constants.SCOUT_HP;
        maxHP = hp;
        dmg = Constants.SCOUT_DMG;
        speed = Constants.SCOUT_SPEED;
        rof = Constants.SCOUT_INIT_FIRE_DELAY;
        threatLevel = 3;
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
        hp = maxHP;
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHP;
            hpSlider.value = hp;
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

