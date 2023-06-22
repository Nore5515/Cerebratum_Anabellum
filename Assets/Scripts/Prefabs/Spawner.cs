using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : Structure
{
    public PathManager spawnerPathManager;

    public GameObject prefab;
    public Material redMat;
    public Material blueMat;

    public List<GameObject> unitList = new List<GameObject>();

    public string spawnerTeam = "RED";

    SpawnedUnitStats spawnedUnitStats = new SpawnedUnitStats();

    GameObject canvas;
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

        if (spawnerTeam == "RED")
        {
            spawnerPathManager.SetPathMat(redMat);
        }
        else
        {
            spawnerPathManager.SetPathMat(blueMat);
        }

        naniteGenPrefab = Resources.Load(naniteGenPrefab_path) as GameObject;
        canvas = GameObject.Find("Canvas");

        AttemptInitializeUI();

        IEnumerator coroutine = SpawnPrefab();
        StartCoroutine(coroutine);

        type = "spawn";
        Debug.Log("SPAWN TEAM! : " + spawnerTeam);
        if (spawnerTeam == "BLUE")
        {
            Debug.Log("Blue path time");
            spawnerPathManager.AI_DrawPath(this.transform.position);
        }
    }

    private void AttemptInitializeUI()
    {
        if (this.gameObject.transform.Find("UI") != null)
        {
            InitalizeUI();
            SetUIVisible(false);
            selected = false;
        }
    }

    public void LateStart()
    {
        Debug.Log("LATE START!");
        spawnedUnitStats.ResetToStartingStats();

        naniteGenPrefab = Resources.Load(naniteGenPrefab_path) as GameObject;
        canvas = GameObject.Find("Canvas");

        AttemptInitializeUI();

        IEnumerator coroutine = SpawnPrefab();
        StartCoroutine(coroutine);
    }

    private void InitalizeUI()
    {
        GameObject ui = this.gameObject.transform.Find("UI").gameObject;
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
        spawnerPathManager.ClearPoints(unitList);
        SetIsDrawable(true);
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
        spawnerPathManager.pathDrawingMode = _newMode;
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
    // TODO: MOVE THIS TO PATH MANAGER
    public int DrawPathSphereAtPoint(Vector3 point, ref Slider pathBar)
    {
        GameObject newPathPoint;
        ClearNullInstances();

        if (spawnerPathManager.pathSpheres.Count <= maxPathLength)
        {
            // Create and add a team path marker.
            newPathPoint = spawnerPathManager.CreatePathMarker(new PathMarkerModel(spawnerTeam, point));
        }
        else
        {
            return -1;
        }

        if (spawnerTeam == "RED")
        {
            UpdateSlider(ref pathBar);
        }

        spawnerPathManager.AddPathMarkerToPathSpheres(newPathPoint);
        AddPathPointToAlliedUnits(newPathPoint);

        return spawnerPathManager.pathSpheres.Count;
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
        // slider.value = (1.0f) * pathSpheres.Count / maxPathLength;
        slider.value = (1.0f) * spawnerPathManager.pathSpheres.Count / maxPathLength;
        // If we hit the max count, color the bar red.
        if (spawnerPathManager.pathSpheres.Count == maxPathLength)
        {
            TintSliderRed(ref slider);
        }
    }

    private void TintSliderRed(ref Slider slider)
    {
        Color red = new Color(233f / 255f, 80f / 255f, 55f / 255f);
        slider.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = red;
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
        obj.GetComponent<Unit>().Initalize(spawnerPathManager.pathSpheres, spawnerTeam, spawnedUnitStats.fireDelay, spawnedUnitStats.unitRange);
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
}