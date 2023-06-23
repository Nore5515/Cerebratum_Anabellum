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
    public Material spawnTeamMat;

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

        spawnTeamMat = (spawnerTeam == "RED") ? redMat : blueMat;

        spawnerPathManager.SetPathMat(spawnTeamMat);

        naniteGenPrefab = Resources.Load(naniteGenPrefab_path) as GameObject;
        canvas = GameObject.Find("Canvas");

        AttemptInitializeUI();

        IEnumerator coroutine = SpawnPrefab();
        StartCoroutine(coroutine);

        SpawnDeclaration();
    }

    private void SpawnDeclaration()
    {
        type = "spawn";
        if (spawnerTeam == "BLUE")
        {
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
        spawnrateButton.onClick.AddListener(delegate { AttemptUpgradeSpawnRate(); });
        firerateButton.onClick.AddListener(delegate { AttemptUpgradeFireRate(); });
        rangeButton.onClick.AddListener(delegate { AttemptUpgradeRange(); });
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
        return spawnrateButton.gameObject.activeSelf;
    }

    public void EnableDrawable()
    {
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

        if (spawnerPathManager.pathSpheres.Count > maxPathLength) return -1;
        newPathPoint = spawnerPathManager.CreatePathMarker(new PathMarkerModel(spawnerTeam, point));

        UpdateSlider(ref pathBar);

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

    void BotSpendPoints()
    {
        switch (Random.Range(1, 3))
        {
            case 3:
                AttemptUpgradeFireRate();
                break;
            case 2:
                AttemptUpgradeRange();
                break;
            case 1:
                AttemptUpgradeSpawnRate();
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

        obj.GetComponent<Unit>().Initalize(spawnerPathManager.pathSpheres, spawnerTeam, spawnedUnitStats);
        obj.GetComponent<MeshRenderer>().material = spawnTeamMat;
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

    public void AttemptUpgradeSpawnRate()
    {
        if (spawnedUnitStats.spawnTime < spawnedUnitStats.MAX_UNIT_SPAWN_RATE) return;
        if (deductTeamPoints(1))
        {
            UpgradeSpawnRate();
        }
    }

    public void AttemptUpgradeFireRate()
    {
        if (spawnedUnitStats.fireDelay < spawnedUnitStats.MAX_UNIT_FIRE_RATE) return;
        if (deductTeamPoints(1))
        {
            UpgradeFireRate();
        }
    }

    public void AttemptUpgradeRange()
    {
        if (spawnedUnitStats.unitRange > spawnedUnitStats.MAX_UNIT_RANGE) return;
        if (deductTeamPoints(1))
        {
            UpgradeRange();
        }
    }

    private void UpgradeSpawnRate()
    {
        spawnedUnitStats.spawnTime += Constants.INF_SPAWN_TIME_UPGRADE_AMOUNT;
        RecalculateFill();
        if (spawnedUnitStats.spawnTime < Constants.INF_MIN_SPAWN_TIME)
        {
            spawnrateButton.interactable = false;
        }
    }

    private void UpgradeFireRate()
    {
        spawnedUnitStats.fireDelay += Constants.INF_FIRE_RATE_UPGRADE_AMOUNT;
        RecalculateFill();
        if (spawnedUnitStats.fireDelay < Constants.INF_MIN_FIRE_DELAY)
        {
            firerateButton.interactable = false;
        }
    }

    private void UpgradeRange()
    {
        spawnedUnitStats.unitRange += Constants.INF_RANGE_UPGRADE_AMOUNT;
        RecalculateFill();
        // rangeFill.fillAmount = calculateFill(spawnedUnitStats.startingUnitRange, 6.5f, spawnedUnitStats.unitRange);
        if (spawnedUnitStats.unitRange > Constants.INF_MAX_RANGE)
        {
            rangeButton.interactable = false;
        }
    }

    private void RecalculateFill()
    {
        spawnFill.fillAmount = calculateFill(spawnedUnitStats.startingSpawnTime, 0.5f, spawnedUnitStats.spawnTime);
        fireRateFill.fillAmount = calculateFill(spawnedUnitStats.startingFireDelay, 0.25f, spawnedUnitStats.fireDelay);
        rangeFill.fillAmount = calculateFill(spawnedUnitStats.startingUnitRange, 6.5f, spawnedUnitStats.unitRange);
    }

    public bool deductTeamPoints(int cost)
    {
        return TeamStats.AttemptPointDeductionFromTeam(cost, spawnerTeam);
    }
}