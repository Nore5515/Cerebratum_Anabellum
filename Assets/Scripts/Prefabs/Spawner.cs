using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : Structure
{
    public GameObject prefab;
    public Material redMat;
    public Material blueMat;

    public List<GameObject> unitList = new List<GameObject>();

    public List<GameObject> paths;
    private GameObject chosenPath;

    public string spawnerTeam = "RED";

    public class SpawnedUnitStats
    {
        public float startingFireDelay = 2.0f;
        public float startingUnitRange = 3.0f;
        public float startingSpawnTime = 3.0f;
        public float MAX_UNIT_FIRE_RATE = 0.5f;
        public float MAX_UNIT_RANGE = 6.0f;
        public float MAX_UNIT_SPAWN_RATE = 1.0f;
        public float fireDelay;
        public float unitRange;
        public float spawnTime;

        public void ResetToStartingStats()
        {
            fireDelay = startingFireDelay;
            unitRange = startingUnitRange;
            spawnTime = startingSpawnTime;
        }
    }

    SpawnedUnitStats spawnedUnitStats = new SpawnedUnitStats();

    // NEW STUFF
    public List<GameObject> pathSpheres = new List<GameObject>();
    GameObject canvas;
    public GameObject pathMarker;
    string path = "Asset_PathMarker";
    public GameObject spawnerUI;
    public bool pathDrawingMode = false;
    public GameObject naniteGenPrefab;
    string naniteGenPrefab_path = "Asset_NaniteGen";

    // UI
    Button spawnrateButton;
    Button firerateButton;
    Button rangeButton;
    Button pathButton;
    // Button deleteButton;

    public Image rangeFill;
    public Image fireRateFill;
    public Image spawnFill;

    float rangeStep;
    float firerateStep;
    float spawnStep;

    bool selected = false;

    public int maxPathLength;

    void Start()
    {
        Debug.Log("!START!");
        spawnedUnitStats.ResetToStartingStats();

        pathMarker = Resources.Load(path) as GameObject;
        naniteGenPrefab = Resources.Load(naniteGenPrefab_path) as GameObject;
        canvas = GameObject.Find("Canvas");

        InitializeUI();

        IEnumerator coroutine = SpawnPrefab();
        StartCoroutine(coroutine);

        type = "spawn";
        if (spawnerTeam == "BLUE")
        {
            AI_DrawPath(this.transform.position);
        }
    }

    private void InitializeUI()
    {
        if (this.gameObject.transform.Find("UI") != null)
        {
            spawnerUI = this.gameObject.transform.Find("UI").gameObject;
            // Debug.Log(spawnerUI);
            InitalizeUI(spawnerUI);
            SetUIVisible(false);
            selected = false;
        }
        else
        {
            spawnerUI = null;
        }
    }

    public void LateStart()
    {
        Debug.Log("LATE START!");
        spawnedUnitStats.ResetToStartingStats();

        pathMarker = Resources.Load(path) as GameObject;
        naniteGenPrefab = Resources.Load(naniteGenPrefab_path) as GameObject;
        canvas = GameObject.Find("Canvas");

        InitializeUI();

        IEnumerator coroutine = SpawnPrefab();
        StartCoroutine(coroutine);
    }

    public void SpawnerPickPath()
    {

    }

    private void InitalizeUI(GameObject ui)
    {
        ui.GetComponent<Canvas>().worldCamera = Camera.main;
        GameObject upgrades = ui.transform.Find("Upgrades").gameObject;

        SetButtonInstances(upgrades, ui);
        AddButtonListeners();

        SetFills(ui);

        ResetUpgradeFillAmounts();
    }

    private void SetButtonInstances(GameObject upgrades, GameObject ui)
    {
        spawnrateButton = upgrades.transform.Find("RSpawn").gameObject.GetComponent<Button>();
        firerateButton = upgrades.transform.Find("RFireRate").gameObject.GetComponent<Button>();
        rangeButton = upgrades.transform.Find("RRange").gameObject.GetComponent<Button>();
        pathButton = ui.transform.Find("RDraw").gameObject.GetComponent<Button>();
    }

    private void AddButtonListeners()
    {
        spawnrateButton.onClick.AddListener(delegate { IncreaseSpawnRate(); });
        firerateButton.onClick.AddListener(delegate { IncreaseFireRate(); });
        rangeButton.onClick.AddListener(delegate { IncreaseRange(); });
        pathButton.onClick.AddListener(delegate { EnableDrawable(); });
    }

    private void ResetUpgradeFillAmounts()
    {
        rangeFill.fillAmount = 0;
        fireRateFill.fillAmount = 0;
        spawnFill.fillAmount = 0;
    }

    private void SetFills(GameObject ui)
    {
        GameObject infhutCanvas = ui.transform.Find("Canvas").gameObject;
        rangeFill = infhutCanvas.transform.Find("range").gameObject.GetComponent<Image>();
        fireRateFill = infhutCanvas.transform.Find("firerate").gameObject.GetComponent<Image>();
        spawnFill = infhutCanvas.transform.Find("spawn").gameObject.GetComponent<Image>();
    }

    public void RemoveSpawner()
    {
        // if (!rootSpawner)
        if (true)
        {
            if (spawnerTeam == "RED")
            {
                StartCoroutine(removeSpawn());
                // Debug.Log("Main:" + SpawnerTracker.redSpawnerObjs.Count);
                TeamStats.RedPoints += 10;
                // Destroy(this.gameObject);
                this.gameObject.SetActive(false);
            }
        }
    }

    public IEnumerator removeSpawn()
    {
        // SpawnerTracker.redSpawnerObjs.Remove(this.gameObject);
        SpawnerTracker.redSpawnerObjs.RemoveAt(1);
        // Debug.Log("Threat:" + SpawnerTracker.redSpawnerObjs.Count);
        yield return 0;
    }

    // For these two, we only use spawnrateButton.
    // This is because, if it doesn't have spawnrate...
    // ...it won't have anything else :\
    public void SetUIVisible(bool isVis)
    {
        selected = isVis;
        if (spawnrateButton != null)
        {
            // Debug.Log("Flippin' UI to " + isVis);
            spawnrateButton.gameObject.SetActive(isVis);
            firerateButton.gameObject.SetActive(isVis);
            rangeButton.gameObject.SetActive(isVis);
            pathButton.gameObject.SetActive(isVis);
        }
        else
        {
            Debug.LogError("SPAWN RATE BUTTON");
        }
    }
    public bool GetUIVisible()
    {
        // Debug.Log("Spawnratebutton: " + spawnrateButton.gameObject.activeSelf);
        return spawnrateButton.gameObject.activeSelf;
    }

    // TODO fuck it ill figure it out tomorrow
    // um
    // make it so that when you push
    // THE BUTTON ON SPAWNER
    // it tells the cube maker to start making cubes
    // but also try to avoid assiging CM? I'm kinda likin the top-down heiarchy
    public void EnableDrawable()
    {
        Debug.Log("DRAWABLE ENABLED!");
        ClearPoints();
        SetIsDrawable(true);
    }

    public bool GetIsDrawable()
    {
        return pathDrawingMode;
    }

    public void SetIsDrawable(bool _newMode)
    {
        if (_newMode == true)
        {
            DimHut();
        }
        else
        {
            UndimHut();
        }
        pathDrawingMode = _newMode;
    }

    private void DimHut()
    {
        Color dim = new Color(166f / 255f, 166f / 255f, 166f / 255f);
        this.transform.Find("InfHut").GetComponent<SpriteRenderer>().color = dim;
    }

    private void UndimHut()
    {
        Color white = new Color(255f / 255f, 255f / 255f, 255f / 255f);
        this.transform.Find("InfHut").GetComponent<SpriteRenderer>().color = white;
    }

    // Returns number of pathSpheres in path now.
    public int DrawPathSphereAtPoint(Vector3 point, ref Slider pathBar)
    {
        GameObject newPathPoint;
        ClearNullInstances();

        if (pathSpheres.Count <= maxPathLength)
        {
            // Create and add a team path marker.
            newPathPoint = CreatePathMarker(new PathMarkerModel(spawnerTeam, point));
            // newPathPoint = AddPathMarkerToPathSpheres(new PathMarkerModel(spawnerTeam, point));
        }
        else
        {
            return -1;
        }

        if (spawnerTeam == "RED")
        {
            UpdateSlider(ref pathBar);
        }

        AddPathMarkerToPathSpheres(newPathPoint);
        AddPathPointToAlliedUnits(newPathPoint);

        return pathSpheres.Count;
    }

    // Update each unit instance with the new point IF they match the team
    private void AddPathPointToAlliedUnits(GameObject newPathPoint)
    {
        foreach (GameObject unit in unitList)
        {
            if (unit.GetComponent<Unit>().unitTeam == spawnerTeam)
            {
                unit.GetComponent<Unit>().AddPoint(newPathPoint);
            }
        }
    }

    private void UpdateSlider(ref Slider slider)
    {
        // Convert it to a float.
        slider.value = (1.0f) * pathSpheres.Count / maxPathLength;
        // If we hit the max count, color the bar red.
        if (pathSpheres.Count == maxPathLength)
        {
            TintSliderRed(ref slider);
        }
    }

    private void TintSliderRed(ref Slider slider)
    {
        Color red = new Color(233f / 255f, 80f / 255f, 55f / 255f);
        slider.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = red;
    }

    // TODO: Also have this only happen when EnableDrawable is pressed.
    public void ClearPoints()
    {
        while (pathSpheres.Count > 0)
        {
            RemovePoint(pathSpheres[0]);
        }
    }

    void PickAndCreatePath(string color)
    {
        SelectRandomPath();
        foreach (Transform orbTransform in chosenPath.transform.GetComponentsInChildren<Transform>())
        {
            if (orbTransform.position != new Vector3(0, 0.5f, 0) && orbTransform.position != new Vector3(0, 0.0f, 0))
            {
                GameObject pathMarker = CreatePathMarker(new PathMarkerModel(color, orbTransform.position));
                AddPathMarkerToPathSpheres(pathMarker);
            }
        }
    }

    public void SelectRandomPath()
    {
        chosenPath = paths[Random.Range(0, paths.Count)];
    }

    public void AddPathMarkerToPathSpheres(GameObject pathMarker)
    {
        pathSpheres.Add(pathMarker);
    }

    private GameObject CreatePathMarker(PathMarkerModel pathMarkerValues)
    {
        if (pathMarkerValues.color == "RED")
        {
            return InstantiateRedPathMarkerAtPoint(pathMarkerValues.pathMarkerLoc);
        }
        else
        {
            return InstantiateBluePathMarkerAtPoint(pathMarkerValues.pathMarkerLoc);
        }
    }

    // Leaving this here as a relic. I find it amusing.
    // TODO: This feels smart but i dont know why
    // It was :)
    private GameObject InstantiateRedPathMarkerAtPoint(Vector3 pathMarkerPoint)
    {
        GameObject redPathMarker = Instantiate(pathMarker, pathMarkerPoint, Quaternion.identity) as GameObject;
        redPathMarker.GetComponent<MeshRenderer>().material = redMat;
        return redPathMarker;
    }

    private GameObject InstantiateBluePathMarkerAtPoint(Vector3 pathMarkerPoint)
    {
        GameObject bluePathMarker = Instantiate(pathMarker, pathMarkerPoint, Quaternion.identity) as GameObject;
        bluePathMarker.GetComponent<MeshRenderer>().material = blueMat;
        bluePathMarker.GetComponent<MeshRenderer>().enabled = false;
        return bluePathMarker;
    }

    void AI_DrawPath(Vector3 position)
    {
        if (paths.Count > 0)
        {
            PickAndCreatePath("BLUE");
        }
        else
        {
            GameObject obj = Instantiate(pathMarker, position, Quaternion.identity) as GameObject;
            obj.GetComponent<MeshRenderer>().material = redMat;
            pathSpheres.Add(obj);
        }
    }

    void AI_SpendPoints()
    {
        switch (Random.Range(1, 3))
        {
            case 3:
                IncreaseFireRate();
                break;
            case 2:
                IncreaseRange();
                break;
            case 1:
                IncreaseSpawnRate();
                break;
            default:
                Debug.Log("ERROR IN AI POINT SPENDING");
                break;
        }
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

    public void CreateNewSpawner()
    {
        Debug.Log("Creating new spawner! Count is : " + SpawnerTracker.redSpawnerObjs.Count);
        if (SpawnerTracker.redSpawnerObjs.Count <= 2)
        {
            if (TeamStats.RedPoints >= 10)
            {
                TeamStats.RedPoints -= 10;
                Debug.Log("New spawner about to be added. Count is : " + SpawnerTracker.redSpawnerObjs.Count);
                Vector3 newPos = SpawnerTracker.redSpawnerObjs[0].transform.position;
                newPos.z += (16.0f * (SpawnerTracker.redSpawnerObjs.Count - 1.5f));
                GameObject newObj = Instantiate(SpawnerTracker.redSpawnerObjs[0], newPos, Quaternion.identity) as GameObject;
                newObj.GetComponent<Structure>().type = "spawn";
                SpawnerTracker.redSpawnerObjs.Add(newObj);
                Debug.Log("New spawner added. Count is : " + SpawnerTracker.redSpawnerObjs.Count);
            }
        }
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

        // TODO: Pass in unit stat object
        obj.GetComponent<Unit>().Initalize(pathSpheres, spawnerTeam, spawnedUnitStats.fireDelay, spawnedUnitStats.unitRange);
        if (spawnerTeam == "RED")
        {
            obj.GetComponent<MeshRenderer>().material = redMat;
        }
        else
        {
            obj.GetComponent<MeshRenderer>().material = blueMat;
        }
    }

    IEnumerator SpawnPrefab()
    {
        yield return new WaitForSeconds(spawnedUnitStats.spawnTime);
        ClearNullInstances();

        InstantiateUnit(prefab);

        StartCoroutine(SpawnPrefab());
    }

    float calculateFill(float min, float max, float target)
    {
        float diff = (max - min);
        float result = (target - min) / diff;
        // Debug.Log("Min/Max: " + min + "/" + max + " with a target of " + target + " = " + result);
        return result;
    }

    public void IncreaseSpawnRate()
    {
        if (spawnedUnitStats.spawnTime >= spawnedUnitStats.MAX_UNIT_SPAWN_RATE)
        {
            if (deductTeamPoints(1))
            {
                spawnedUnitStats.spawnTime -= 0.5f;
                spawnFill.fillAmount = calculateFill(spawnedUnitStats.startingSpawnTime, 0.5f, spawnedUnitStats.spawnTime);
            }
        }
        if (spawnedUnitStats.spawnTime < 1.0f)
        {
            spawnrateButton.interactable = false;
        }
    }

    public void IncreaseFireRate()
    {
        if (spawnedUnitStats.fireDelay >= spawnedUnitStats.MAX_UNIT_FIRE_RATE)
        {
            if (deductTeamPoints(1))
            {
                spawnedUnitStats.fireDelay -= 0.25f;
                fireRateFill.fillAmount = calculateFill(spawnedUnitStats.startingFireDelay, 0.25f, spawnedUnitStats.fireDelay);
            }
        }
        if (spawnedUnitStats.fireDelay < 0.5f)
        {
            firerateButton.interactable = false;
        }
    }

    public void IncreaseRange()
    {
        if (spawnedUnitStats.unitRange <= spawnedUnitStats.MAX_UNIT_RANGE)
        {
            if (deductTeamPoints(1))
            {
                spawnedUnitStats.unitRange += 0.5f;
                rangeFill.fillAmount = calculateFill(spawnedUnitStats.startingUnitRange, 6.5f, spawnedUnitStats.unitRange);
            }
        }
        if (spawnedUnitStats.unitRange > 6.0f)
        {
            rangeButton.interactable = false;
        }
    }

    public bool deductTeamPoints(int cost)
    {
        return TeamStats.AttemptPointDeductionFromTeam(cost, spawnerTeam);
    }

    // NEW STUFF
    public void RemovePoint(GameObject obj)
    {
        pathSpheres.Remove(obj);
        foreach (GameObject unit in unitList)
        {
            if (unit != null)
            {
                unit.GetComponent<Unit>().RemovePoint(obj);
            }
        }
        GameObject.Destroy(obj);
    }


}