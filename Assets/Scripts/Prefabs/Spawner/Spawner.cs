using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// THIS IS THE SPAWNER OBJ CLASS
// I JUST DONT WANNA REFACTOR THE NAME RN LOL

// Child Scripts:
//      SpawnerUI.cs

public class Spawner : Structure
{
    public PathManager spawnerPathManager;

    public GameObject prefab;
    public Material redMat;
    public Material blueMat;
    public Material spawnTeamMat;

    public List<GameObject> unitList = new List<GameObject>();

    public string spawnerTeam = "RED";

    SpawnedUnitStats spawnedUnitStats = new SpawnedUnitStats();
    SpawnerStatsHandler spawnerStatsHandler;

    public SpawnerUI spawnerUI;

    public int maxPathLength;

    public string unitType = "Infantry";

    void Start()
    {
        spawnedUnitStats.ResetToStartingStats("Infantry");

        spawnerStatsHandler = gameObject.AddComponent<SpawnerStatsHandler>();
        spawnerStatsHandler.Initialize(unitType);

        spawnTeamMat = (spawnerTeam == "RED") ? redMat : blueMat;

        spawnerPathManager.SetPathMat(spawnTeamMat);

        spawnerUI.AttemptInitializeUI();

        IEnumerator coroutine = SpawnPrefab();
        StartCoroutine(coroutine);

        SpawnDeclaration();
    }

    private void SpawnDeclaration()
    {
        type = "spawn";
        if (spawnerTeam == "BLUE")
        {
            spawnerPathManager.AI_DrawPath(transform.position);
        }
    }

    public void LateStart()
    {
        spawnedUnitStats.ResetToStartingStats("Infantry");

        IEnumerator coroutine = SpawnPrefab();
        StartCoroutine(coroutine);
    }

    public void SetUIVisible(bool isVis)
    {
        spawnerUI.SetUIVisible(isVis);
    }

    public bool GetUIVisible()
    {
        return spawnerUI.GetUIVisible();
    }

    // TODO: Move ClearPoints to it's own thing.
    public void EnableDrawable()
    {
        spawnerPathManager.ClearPoints(unitList);
        //spawnerPathManager.UpdatePathlessUnits(unitList);
        SetIsDrawable(true);
    }

    public void DisableDrawable()
    {
        spawnerPathManager.ClearPoints(unitList);
        SetIsDrawable(true);
    }

    public void SetIsDrawable(bool _newMode)
    {
        spawnerUI.HandleNewDrawableState(_newMode, spawnerPathManager);
        spawnerPathManager.pathDrawingMode = _newMode;
        if (_newMode == true)
        {
            DimHut();
        }
        else
        {
            UndimHut();
        }
        
    }

    private void DimHut()
    {
        Color dim = new Color(166f / 255f, 166f / 255f, 166f / 255f);
        transform.Find("InfHut").GetComponent<SpriteRenderer>().color = dim;
    }

    private void UndimHut()
    {
        Color white = new Color(255f / 255f, 255f / 255f, 255f / 255f);
        transform.Find("InfHut").GetComponent<SpriteRenderer>().color = white;
    }

    // Returns number of pathSpheres in path now.
    // TODO: MOVE THIS TO PATH MANAGER
    public int DrawPathSphereAtPoint(Vector3 point, ref Slider pathBar)
    {
        GameObject newPathPoint;
        ClearNullInstances();

        if (spawnerPathManager.pathSpheres.Count > maxPathLength) return -1;
        newPathPoint = spawnerPathManager.CreatePathMarker(new PathMarkerModel(spawnerTeam, point));

        spawnerUI.UpdateSlider(ref pathBar, spawnerPathManager, maxPathLength);

        spawnerPathManager.AddPathMarkerToPathSpheres(newPathPoint);
        return spawnerPathManager.pathSpheres.Count;
    }

    public void ClearNullInstances()
    {
        List<GameObject> nullUnitList = GenerateNullUnitList();
        foreach (GameObject nullUnit in nullUnitList)
        {
            unitList.Remove(nullUnit);
        }
    }

    private List<GameObject> GenerateNullUnitList()
    {
        List<GameObject> nullUnitList = new List<GameObject>();
        foreach (GameObject unit in unitList)
        {
            if (unit == null)
            {
                nullUnitList.Add(unit);
            }
        }
        return nullUnitList;
    }

    public void DEBUG_BreakEcon()
    {
        GameObject.Find("Economy").GetComponent<Economy>().SetCycleMax(1);
    }

    private void InstantiateUnit(GameObject reqPrefab)
    {
        // TODO: When creating, add a new field to SpawnerData to determine unit type to spawn!!!
        GameObject obj = Instantiate(reqPrefab, transform.position, Quaternion.identity);
        unitList.Add(obj);

        if (unitType == "Spider")
        {
            spawnedUnitStats.fireDelay = 2.0f;
            spawnedUnitStats.spawnDelay = 5.0f;
            spawnedUnitStats.unitRange = 8.0f;
        }
        else if (unitType == "Infantry")
        {
            spawnedUnitStats.fireDelay = Constants.INF_INIT_FIRE_DELAY;
            spawnedUnitStats.spawnDelay = Constants.INF_INIT_SPAWN_DELAY;
            spawnedUnitStats.unitRange = Constants.INF_INIT_RANGE;
        }
        else if (unitType == "Scout")
        {
            spawnedUnitStats.fireDelay = Constants.SCOUT_INIT_FIRE_DELAY;
            spawnedUnitStats.spawnDelay = Constants.SCOUT_INIT_SPAWN_DELAY;
            spawnedUnitStats.unitRange = Constants.SCOUT_INIT_RANGE;
        }
        else
        {
            // Just default to infantry for now.
            Debug.Log("Unit not recognized.");
            spawnedUnitStats.fireDelay = Constants.INF_INIT_FIRE_DELAY;
            spawnedUnitStats.spawnDelay = Constants.INF_INIT_SPAWN_DELAY;
            spawnedUnitStats.unitRange = Constants.INF_INIT_RANGE;
        }
        obj.GetComponent<Unit>().Initalize(spawnerPathManager.pathSpheres, spawnerTeam, spawnedUnitStats);
        obj.GetComponent<MeshRenderer>().material = spawnTeamMat;
    }

    IEnumerator SpawnPrefab()
    {
        yield return new WaitForSeconds(spawnedUnitStats.spawnDelay);
        ClearNullInstances();

        InstantiateUnit(prefab);

        StartCoroutine(SpawnPrefab());
    }

    public void AttemptUpgradeSpawnRate()
    {
        if (IsMaxedSpawnRate()) return;
        if (DeductTeamPoints(1))
        {
            UpgradeSpawnRate();
        }
    }

    public bool IsMaxedSpawnRate()
    {
        return spawnerStatsHandler.IsMaxedSpawnRate(spawnedUnitStats);
    }

    public void AttemptUpgradeFireRate()
    {
        if (IsMaxedFireRate()) return;
        if (DeductTeamPoints(1))
        {
            UpgradeFireRate();
        }
    }

    public bool IsMaxedFireRate()
    {
        return spawnerStatsHandler.IsMaxedFireRate(spawnedUnitStats);
    }

    public void AttemptUpgradeRange()
    {
        if (IsMaxedRange()) return;
        if (DeductTeamPoints(1))
        {
            UpgradeRange();
        }
    }

    public bool IsMaxedRange()
    {
        return spawnerStatsHandler.IsMaxedRange(spawnedUnitStats);
    }

    private void UpgradeSpawnRate()
    {
        spawnerStatsHandler.UpgradeSpawnRate(spawnedUnitStats, spawnerUI);
    }

    private void UpgradeFireRate()
    {
        spawnerStatsHandler.UpgradeFireRate(spawnedUnitStats, spawnerUI);
    }

    private void UpgradeRange()
    {
        spawnerStatsHandler.UpgradeRange(spawnedUnitStats, spawnerUI);
    }

    public bool DeductTeamPoints(int cost)
    {
        return TeamStats.AttemptPointDeductionFromTeam(cost, spawnerTeam);
    }

    public void UpdateAwaitingUnits()
    {
        spawnerPathManager.UpdatePathlessUnits(unitList);
    }
}