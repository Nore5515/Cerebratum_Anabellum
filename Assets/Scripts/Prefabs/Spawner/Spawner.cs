using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// THIS IS THE SPAWNER OBJ CLASS
// I JUST DONT WANNA REFACTOR THE NAME RN LOL

// Child Scripts:
//      SpawnerUI.cs

public class Spawner : MonoBehaviour
{
    public SpawnerPathManager spawnerPathManager;

    public string type;

    [SerializeField] GameObject scoutPrefab;
    [SerializeField] GameObject infantryPrefab;
    [SerializeField] GameObject spiderPrefab;
    [SerializeField] GameObject drawButtonCube;
    [SerializeField] GameObject spawnScoutButton;
    [SerializeField] GameObject unitFactoryPrefab;
    [SerializeField] GameObject env_scoutSpawning;
    [SerializeField] GameObject scoutCooldownSlider;
    public GameObject prefab;
    public Material redMat;
    public Material blueMat;
    public Material spawnTeamMat;

    UnitFactory uf;

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

    [SerializeField]
    public int spawnSquadSize = 10;
    public float spawnDelay = 30.0f;

    public float maxScoutSpawnDelay = 15.0f;
    float scoutSpawnDelay = 15.0f;

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
        spawnScoutButton.SetActive(false);

        FetchUnitFactory();

        scoutCooldownSlider.GetComponent<Slider>().maxValue = maxScoutSpawnDelay;
    }

    private void Update()
    {
        if (scoutSpawnDelay > 0.0f)
        {
            scoutSpawnDelay -= Time.deltaTime;
            scoutCooldownSlider.GetComponent<Slider>().value = scoutSpawnDelay;
        }
    }

    void FetchUnitFactory()
    {
        uf = GetSceneUnitFactory();
    }

    UnitFactory GetSceneUnitFactory()
    {
        UnitFactory uf = FindObjectOfType<UnitFactory>();
        if (uf == null)
        {
            // Create Unit Factory and add it to the scene!
            GameObject instance = Instantiate(unitFactoryPrefab, transform.position, transform.rotation);
            instance.transform.SetParent(transform);
            instance.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            return GetSceneUnitFactory();
        }
        return uf;
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
        spawnScoutButton.SetActive(isVis);
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

    private IEnumerator SpawnPrefab(GameObject prefabToSpawn)
    {
        yield return new WaitForSeconds(spawnDelay);
        ClearNullInstances();

        for (int x = 0; x < spawnSquadSize; x++)
        {
            GameObject unitInstance = uf.CreateUnit(unitType, spawnerPathManager.GetPathPoints(), spawnerTeam, transform);
            unitList.Add(unitInstance);
        }

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

    public void SpawnScout()
    {
        GameObject[] canvasObj = GameObject.FindGameObjectsWithTag("scout_spawner");
        if (canvasObj.Length > 0)
        {
            if (scoutSpawnDelay <= 0.0f)
            {
                Debug.Log("Spawning Scout!");
                Vector3 belowSpawner = transform.position;
                belowSpawner = new Vector3(belowSpawner.x, belowSpawner.y - 1.0f, belowSpawner.z);
                if (canvasObj[0].GetComponent<ScoutSpawning>().SpawnScout(belowSpawner))
                {
                    scoutSpawnDelay = maxScoutSpawnDelay;
                }
            }
        }
        else
        {
            Debug.Log("Mkaing Env");
            Instantiate(env_scoutSpawning, transform.position, transform.rotation);
            SpawnScout();
        }
    }
}