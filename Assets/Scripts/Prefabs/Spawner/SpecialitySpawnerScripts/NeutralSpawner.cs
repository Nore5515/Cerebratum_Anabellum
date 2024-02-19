using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// THIS IS THE SPAWNER OBJ CLASS
// I JUST DONT WANNA REFACTOR THE NAME RN LOL

// Child Scripts:
//      SpawnerUI.cs

public class NeutralSpawner : MonoBehaviour
{
    public SpawnerPathManager spawnerPathManager;

    public string type;

    [SerializeField] GameObject scoutPrefab;
    [SerializeField] GameObject infantryPrefab;
    [SerializeField] GameObject spiderPrefab;
    public GameObject prefab;
    public Material redMat;
    public Material blueMat;
    public Material spawnTeamMat;

    public List<GameObject> unitList = new List<GameObject>();

    public string spawnerTeam = "NEUTRAL";

    SpawnedUnitStats spawnedUnitStats = new SpawnedUnitStats();
    SpawnerStatsHandler spawnerStatsHandler;

    public Slider captureSlider;
    public DetectorSphere detectorSphere;

    public int maxPathLength;

    public string unitType = Constants.INF_TYPE;

    bool spawningIsValid = false;

    public List<GameObject> neutralPath;

    void Start()
    {
        InitializeUnitType(unitType);

        spawnTeamMat = (spawnerTeam == "RED") ? redMat : blueMat;

        spawnerPathManager.SetPathMat(spawnTeamMat);

        IEnumerator coroutine = SpawnPrefab(prefab);
        StartCoroutine(coroutine);

        SpawnDeclaration();
    }

    private void Update()
    {
        if (detectorSphere.unitInRange)
        {
            //Debug.Log("ON");
            spawningIsValid = true;
            captureSlider.transform.GetChild(0).GetComponent<Image>().color = new Color(0, 255, 0);

        }
        else
        {
            //Debug.Log("OFF");
            spawningIsValid = false;
            captureSlider.transform.GetChild(0).GetComponent<Image>().color = new Color(255, 255, 255);
        }


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
        if (unitTypeString == Constants.INF_TYPE)
        {
            return infantryPrefab;
        }
        else if (unitTypeString == Constants.SCOUT_TYPE)
        {
            return scoutPrefab;
        }
        else if (unitTypeString == "Spider")
        {
            return spiderPrefab;
        }
        else
        {
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
            // Just default to infantry for now.
            Debug.Log("Unit not recognized.");
            spawnedUnitStats.fireDelay = Constants.INF_INIT_FIRE_DELAY;
            spawnedUnitStats.spawnDelay = Constants.INF_INIT_SPAWN_DELAY;
            spawnedUnitStats.unitRange = Constants.INF_INIT_RANGE;
        }
        obj.GetComponent<Unit>().Initalize(neutralPath, "RED", spawnedUnitStats);
        obj.GetComponent<MeshRenderer>().material = spawnTeamMat;
    }

    IEnumerator SpawnPrefab(GameObject prefabToSpawn)
    {
        yield return new WaitForSeconds(spawnedUnitStats.spawnDelay);
        ClearNullInstances();

        if (spawningIsValid)
        {
            InstantiateUnit(prefabToSpawn);
        }

        StartCoroutine(SpawnPrefab(prefabToSpawn));
    }

    public void UpdateAwaitingUnits()
    {
        spawnerPathManager.UpdatePathlessUnits(unitList);
    }
}