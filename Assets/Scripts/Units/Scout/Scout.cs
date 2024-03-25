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
        Constants.MIN_DIST_TO_MOVEMENT_DEST = 1;

        // Core stat initialization
        unitStats.hp = Constants.SCOUT_HP;
        unitStats.maxHP = unitStats.hp;
        unitStats.dmg = Constants.SCOUT_DMG;
        unitStats.speed = Constants.SCOUT_SPEED;
        unitStats.rof = Constants.SCOUT_INIT_FIRE_DELAY;
        unitStats.threatLevel = 3;
        unitStats.unitType = "Scout";

        if (debugMode)
        {
            SpawnedUnitStats debugSpawnedUnitStats = new SpawnedUnitStats();
            debugSpawnedUnitStats.fireDelay = Constants.SCOUT_INIT_FIRE_DELAY;
            debugSpawnedUnitStats.spawnDelay = Constants.SCOUT_INIT_SPAWN_DELAY;
            debugSpawnedUnitStats.unitRange = Constants.SCOUT_INIT_RANGE;
            Initalize(new List<Vector3>(), "RED", debugSpawnedUnitStats);
        }
    }

    int GetCrateCount()
    {
        GameObject[] crates = GameObject.FindGameObjectsWithTag("crate");
        return crates.Length;
    }

    GameObject GetTeamHQ()
    {
        GameObject[] hqs = GameObject.FindGameObjectsWithTag("hq");
        foreach (GameObject hq in hqs)
        {
            if (hq.GetComponent<HQObject>().team == unitStats.unitTeam)
            {
                return hq;
            }
        }
        Debug.LogError("Could not find team hq");
        return null;
    }

    GameObject GetNearestCrate()
    {
        GameObject[] crates = GameObject.FindGameObjectsWithTag("crate");
        GameObject nearest = crates[0];
        float shortestDist = int.MaxValue;
        Vector3 hqPos = GetTeamHQ().transform.position;
        foreach (GameObject crate in crates)
        {
            float newDist = Vector3.Distance(crate.transform.position, hqPos);
            if (newDist < shortestDist)
            {
                nearest = crate;
                shortestDist = newDist;
            }
        }
        return nearest;
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

    public override void Die()
    {
        if (unitStats.unitTeam == Constants.RED_TEAM)
        {
            TeamStats.RedScouts--;
        }
        else if (unitStats.unitTeam == Constants.BLUE_TEAM)
        {
            TeamStats.BlueScouts--;
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
        if (GetCrateCount() > 0)
        {
            Debug.Log("Crate Count is > 0!");
            if (unitPointHandler.pointVectors.Count == 0)
            {
                Debug.Log("Adding point for scout!!");
                AddPoint(GetNearestCrate().transform.position);
            }
        }
        MovementUpdate();
        IdleUpdate();
    }
}

