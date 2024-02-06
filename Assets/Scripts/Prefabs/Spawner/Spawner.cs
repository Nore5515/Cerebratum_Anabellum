using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// THIS IS THE SPAWNER OBJ CLASS
// I JUST DONT WANNA REFACTOR THE NAME RN LOL

// Child Scripts:
//      SpawnerUI.cs

public class Spawner : Structure
{
    public SpawnerPathManager spawnerPathManager;

    [SerializeField] GameObject scoutPrefab;
    [SerializeField] GameObject infantryPrefab;
    [SerializeField] GameObject spiderPrefab;
    [SerializeField] GameObject drawButtonCube;
    public GameObject prefab;
    public Material redMat;
    public Material blueMat;
    public Material spawnTeamMat;

    [SerializeField]
    GameObject selectCircle;

    public List<GameObject> unitList = new List<GameObject>();

    public string spawnerTeam = "RED";

    SpawnedUnitStats spawnedUnitStats = new SpawnedUnitStats();
    SpawnerStatsHandler spawnerStatsHandler;

    public SpawnerUI spawnerUI;

    public int maxPathLength;

    public string unitType = Constants.INF_TYPE;

    public bool testMode_noPossession = false;

    void Start()
    {
        InitializeUnitType(unitType);
        InitializeTeam();

        spawnerUI.AttemptInitializeUI();

        IEnumerator coroutine = SpawnPrefab(prefab);
        StartCoroutine(coroutine);

        SpawnDeclaration();

        selectCircle.SetActive(false);
        drawButtonCube.SetActive(false);
    }

    void InitializeTeam()
    {
        spawnTeamMat = (spawnerTeam == "RED") ? redMat : blueMat;

        spawnerPathManager.SetPathMat(spawnTeamMat);
    }

    void InitializeUnitType(string newUnitType)
    {
        unitType = newUnitType;
        prefab = GetPrefabFromUnitString(unitType);
        spawnedUnitStats.ResetToStartingStats(unitType);

        spawnerStatsHandler = gameObject.AddComponent<SpawnerStatsHandler>();
        spawnerStatsHandler.Initialize(unitType);
    }

    GameObject GetPrefabFromUnitString(string unitTypeString)
    {
        switch (unitTypeString)
        {
            case ("Infantry"):
                return infantryPrefab;
            case ("Scout"):
                return scoutPrefab;
            case ("Spider"):
                return spiderPrefab;
            default:
                Debug.LogError("UNKNOWN UNIT TYPE STRING");
                return null;
        }
    }

    private void SpawnDeclaration()
    {
        type = "spawn";
        if (spawnerTeam == "BLUE")
        {
            spawnerPathManager.AI_DrawPath(transform.position);
        }
    }

    public void LateStart(string newUnitType)
    {
        InitializeUnitType(newUnitType);

        IEnumerator coroutine = SpawnPrefab(prefab);
        StartCoroutine(coroutine);
    }

    public void SetUIVisible(bool isVis)
    {
        spawnerUI.SetUIVisible(isVis);
        drawButtonCube.SetActive(isVis);
        selectCircle.SetActive(isVis);
    }

    // THIS IS THE ON-DRAW-BUTTON-PRESSED-FUNCTION
    public void EnableDrawable()
    {
        spawnerPathManager.ClearPoints(unitList);
        SetIsDrawable(true);
    }

    public void DisableDrawable()
    {
        spawnerPathManager.ClearPoints(unitList);
        SetIsDrawable(false);
    }

    public void SetIsDrawable(bool _newMode)
    {
        spawnerUI.HandleNewDrawableState(_newMode, spawnerPathManager);
        spawnerPathManager.pathDrawingMode = _newMode;
        if (_newMode) DimHut();
        else UndimHut();
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
    public int DrawPathSphereAtPoint(Vector3 point)
    {
        ClearNullInstances(); // Necessary call for cleanup.
        if (SphereCountIsMaxed()) return -1;

        PathMarkerModel newMarkerModel = new PathMarkerModel(spawnerTeam, point);
        GameObject newPathPoint = spawnerPathManager.CreatePathMarker(newMarkerModel);

        // Update various subclasses
        //spawnerUI.UpdateSlider(ref pathBar, spawnerPathManager, maxPathLength);
        spawnerPathManager.AddPathMarkerToPathSpheres(newPathPoint);

        Debug.Log("Drawing sphere!");

        return spawnerPathManager.pathSpheres.Count;
    }

    private bool SphereCountIsMaxed() { return spawnerPathManager.pathSpheres.Count > maxPathLength; }

    public bool IsPathLongerThanMaxLength()
    {
        return spawnerPathManager.pathSpheres.Count > maxPathLength;
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

    private void InstantiateUnit(GameObject reqPrefab)
    {
        GameObject obj = Instantiate(reqPrefab, transform.position, Quaternion.identity);
        unitList.Add(obj);

        if (unitType == "Spider")
        {
            spawnedUnitStats.fireDelay = Constants.SPIDER_INIT_FIRE_DELAY;
            spawnedUnitStats.spawnDelay = Constants.SPIDER_INIT_SPAWN_DELAY;
            spawnedUnitStats.unitRange = Constants.SPIDER_INIT_RANGE;
        }
        else if (unitType == Constants.INF_TYPE)
        {
            spawnedUnitStats.fireDelay = Constants.INF_INIT_FIRE_DELAY;
            spawnedUnitStats.spawnDelay = Constants.INF_INIT_SPAWN_DELAY;
            spawnedUnitStats.unitRange = Constants.INF_INIT_RANGE;
        }
        else if (unitType == Constants.SCOUT_TYPE)
        {
            spawnedUnitStats.fireDelay = Constants.SCOUT_INIT_FIRE_DELAY;
            spawnedUnitStats.spawnDelay = Constants.SCOUT_INIT_SPAWN_DELAY;
            spawnedUnitStats.unitRange = Constants.SCOUT_INIT_RANGE;
        }
        else
        {
            Debug.LogError("Unit not recognized.");
        }
        obj.GetComponent<Unit>().testMode_noPossession = testMode_noPossession;
        obj.GetComponent<Unit>().Initalize(spawnerPathManager.pathSpheres, spawnerTeam, spawnedUnitStats);
        obj.GetComponent<MeshRenderer>().material = spawnTeamMat;
    }

    private IEnumerator SpawnPrefab(GameObject prefabToSpawn)
    {
        yield return new WaitForSeconds(spawnedUnitStats.spawnDelay);
        ClearNullInstances();

        InstantiateUnit(prefabToSpawn);

        StartCoroutine(SpawnPrefab(prefabToSpawn));
    }

    public void DEBUG_BreakEcon()
    {
        GameObject.Find("Economy").GetComponent<Economy>().SetCycleMax(1);
    }

    public void AttemptUpgradeSpawnRate()
    {
        if (IsMaxedSpawnRate()) return;
        if (DeductTeamPoints(1)) UpgradeSpawnRate();
    }

    public bool IsMaxedSpawnRate() { return spawnerStatsHandler.IsMaxedSpawnRate(spawnedUnitStats); }

    public void AttemptUpgradeFireRate()
    {
        if (IsMaxedFireRate()) return;
        if (DeductTeamPoints(1)) UpgradeFireRate();
    }

    public bool IsMaxedFireRate() { return spawnerStatsHandler.IsMaxedFireRate(spawnedUnitStats); }

    public void AttemptUpgradeRange()
    {
        if (IsMaxedRange()) return;
        if (DeductTeamPoints(1)) UpgradeRange();
    }

    public bool IsMaxedRange() { return spawnerStatsHandler.IsMaxedRange(spawnedUnitStats); }

    private void UpgradeSpawnRate() { spawnerStatsHandler.UpgradeSpawnRate(spawnedUnitStats, spawnerUI); }

    private void UpgradeFireRate() { spawnerStatsHandler.UpgradeFireRate(spawnedUnitStats, spawnerUI); }

    private void UpgradeRange() { spawnerStatsHandler.UpgradeRange(spawnedUnitStats, spawnerUI); }

    public bool DeductTeamPoints(int cost) { return TeamStats.AttemptPointDeductionFromTeam(cost, spawnerTeam); }

    public void UpdateAwaitingUnits() { spawnerPathManager.UpdatePathlessUnits(unitList); }
}