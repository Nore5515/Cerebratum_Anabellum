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

    public GameObject naniteGenPrefab;
    string naniteGenPrefab_path = "Asset_NaniteGen";

    public SpawnerUI spawnerUI;

    public int maxPathLength;

    void Start()
    {
        spawnedUnitStats.ResetToStartingStats();

        spawnTeamMat = (spawnerTeam == "RED") ? redMat : blueMat;

        spawnerPathManager.SetPathMat(spawnTeamMat);

        naniteGenPrefab = Resources.Load(naniteGenPrefab_path) as GameObject;

        spawnerUI.AttemptInitializeUI();
        //AttemptInitializeUI();

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
        // Debug.Log("LATE START!");
        spawnedUnitStats.ResetToStartingStats();

        if (!spawnerUI.AttemptInitializeUI())
        {
            //Debug.LogError("Could not initialize UI for spawner");
            //return;
        }

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
        GameObject obj = Instantiate(reqPrefab, this.transform.position, Quaternion.identity) as GameObject;
        unitList.Add(obj);

        if (reqPrefab.name.Contains("Spider"))
        {
            spawnedUnitStats.fireDelay = 2.0f;
            spawnedUnitStats.spawnTime = 5.0f;
            spawnedUnitStats.unitRange = 8.0f;
            obj.GetComponent<Unit>().Initalize(spawnerPathManager.pathSpheres, spawnerTeam, spawnedUnitStats);
        }
        else
        {
            obj.GetComponent<Unit>().Initalize(spawnerPathManager.pathSpheres, spawnerTeam, spawnedUnitStats);
        }
        obj.GetComponent<MeshRenderer>().material = spawnTeamMat;
    }

    IEnumerator SpawnPrefab()
    {
        yield return new WaitForSeconds(spawnedUnitStats.spawnTime);
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
        if (spawnedUnitStats.spawnTime < spawnedUnitStats.MAX_UNIT_SPAWN_RATE) return true;
        return false;
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
        if (spawnedUnitStats.fireDelay < spawnedUnitStats.MAX_UNIT_FIRE_RATE) return true;
        return false;
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
        if (spawnedUnitStats.unitRange > spawnedUnitStats.MAX_UNIT_RANGE) return true;
        return false;
    }

    private void UpgradeSpawnRate()
    {
        spawnedUnitStats.spawnTime += Constants.INF_SPAWN_TIME_UPGRADE_AMOUNT;
        spawnerUI.RecalculateFill(spawnedUnitStats);
        if (spawnedUnitStats.spawnTime < Constants.INF_MIN_SPAWN_TIME)
        {
            spawnerUI.spawnrateButton.interactable = false;
        }
    }

    private void UpgradeFireRate()
    {
        spawnedUnitStats.fireDelay += Constants.INF_FIRE_RATE_UPGRADE_AMOUNT;
        spawnerUI.RecalculateFill(spawnedUnitStats);
        if (spawnedUnitStats.fireDelay < Constants.INF_MIN_FIRE_DELAY)
        {
            spawnerUI.firerateButton.interactable = false;
        }
    }

    private void UpgradeRange()
    {
        spawnedUnitStats.unitRange += Constants.INF_RANGE_UPGRADE_AMOUNT;
        spawnerUI.RecalculateFill(spawnedUnitStats);
        // rangeFill.fillAmount = calculateFill(spawnedUnitStats.startingUnitRange, 6.5f, spawnedUnitStats.unitRange);
        if (spawnedUnitStats.unitRange > Constants.INF_MAX_RANGE)
        {
            spawnerUI.rangeButton.interactable = false;
        }
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