using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretUnit : Unit
{
    // Core unit stats
    public float survivalTime = 15.0f;
    public PathHandler cm;
    public Animation anim;
    public bool debugMode = false;

    [SerializeField]
    bool isRed;

    [SerializeField]
    bool isBlue;


    public void CTurret()
    {
        unitObj = gameObject;
        controlDirection = new Vector3(0, 0, 0);
        Constants.MIN_DIST_TO_MOVEMENT_DEST = 1;

        // Core stat initialization
        unitStats.hp = Constants.TUR_HP;
        unitStats.maxHP = unitStats.hp;
        unitStats.dmg = Constants.TUR_DMG;
        unitStats.speed = Constants.TUR_SPEED;
        unitStats.rof = Constants.TUR_INIT_FIRE_DELAY;
        unitStats.threatLevel = 3;
        unitStats.unitType = Constants.TUR_TYPE;

        if (debugMode)
        {
            SpawnedUnitStats debugSpawnedUnitStats = new SpawnedUnitStats();
            debugSpawnedUnitStats.fireDelay = Constants.TUR_INIT_FIRE_DELAY;
            debugSpawnedUnitStats.spawnDelay = Constants.TUR_INIT_SPAWN_DELAY;
            debugSpawnedUnitStats.unitRange = Constants.TUR_INIT_RANGE;
            Initalize(new List<Vector3>(), "RED", debugSpawnedUnitStats);
        }

        if (isRed)
        {
            SpawnedUnitStats debugSpawnedUnitStats = new SpawnedUnitStats();
            debugSpawnedUnitStats.fireDelay = Constants.TUR_INIT_FIRE_DELAY;
            debugSpawnedUnitStats.spawnDelay = Constants.TUR_INIT_SPAWN_DELAY;
            debugSpawnedUnitStats.unitRange = Constants.TUR_INIT_RANGE;
            Initalize(new List<Vector3>(), Constants.RED_TEAM, debugSpawnedUnitStats);
        }
        else if (isBlue)
        {
            SpawnedUnitStats debugSpawnedUnitStats = new SpawnedUnitStats();
            debugSpawnedUnitStats.fireDelay = Constants.TUR_INIT_FIRE_DELAY;
            debugSpawnedUnitStats.spawnDelay = Constants.TUR_INIT_SPAWN_DELAY;
            debugSpawnedUnitStats.unitRange = Constants.TUR_INIT_RANGE;
            Initalize(new List<Vector3>(), Constants.BLUE_TEAM, debugSpawnedUnitStats);
        }
    }
    void Start()
    {
        CTurret();
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

