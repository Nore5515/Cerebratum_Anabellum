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
    // public List<GameObject> nullUnitList = new List<GameObject>();

    public List<GameObject> paths;

    public string spawnerTeam = "RED";
    // TODO: Max max  versions later for better fill count
    public float startingFireDelay = 2.0f;
    public float startingUnitRange = 3.0f;
    public float startingSpawnTime = 3.0f;
    public float fireDelay;
    public float unitRange;
    public float spawnTime;

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
        fireDelay = startingFireDelay;
        unitRange = startingUnitRange;
        spawnTime = startingSpawnTime;

        pathMarker = Resources.Load(path) as GameObject;
        naniteGenPrefab = Resources.Load(naniteGenPrefab_path) as GameObject;
        canvas = GameObject.Find("Canvas");

        InitializeUI();

        IEnumerator coroutine = SpawnPrefab();
        StartCoroutine(coroutine);

        // TODO: What the hell is this
        // Root spawner behaves different than every other.
        if (SpawnerTracker.redRootSpawner == null && spawnerTeam == "RED")
        {
            SpawnerTracker.redRootSpawner = this.gameObject;
            SpawnerTracker.redSpawnerObjs.Add(this.gameObject);
            type = "spawn";
        }
        if (SpawnerTracker.blueRootSpawner == null && spawnerTeam == "BLUE")
        {
            SpawnerTracker.blueRootSpawner = this.gameObject;
            SpawnerTracker.blueSpawnerObjs.Add(this.gameObject);
            if (this.gameObject != null)
            {
                AI_DrawPath(SpawnerTracker.blueSpawnerObjs[0].transform.position);
            }
            type = "spawn";
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
        fireDelay = startingFireDelay;
        unitRange = startingUnitRange;
        spawnTime = startingSpawnTime;

        pathMarker = Resources.Load(path) as GameObject;
        naniteGenPrefab = Resources.Load(naniteGenPrefab_path) as GameObject;
        canvas = GameObject.Find("Canvas");

        InitializeUI();

        IEnumerator coroutine = SpawnPrefab();
        StartCoroutine(coroutine);
    }

    private void InitalizeUI(GameObject ui)
    {
        // Debug.Log("What");
        ui.GetComponent<Canvas>().worldCamera = Camera.main;
        // Debug.Log("What2");
        GameObject upgrades = ui.transform.Find("Upgrades").gameObject;
        // Debug.Log("What3");

        // Buttons
        spawnrateButton = upgrades.transform.Find("RSpawn").gameObject.GetComponent<Button>();
        firerateButton = upgrades.transform.Find("RFireRate").gameObject.GetComponent<Button>();
        rangeButton = upgrades.transform.Find("RRange").gameObject.GetComponent<Button>();
        pathButton = ui.transform.Find("RDraw").gameObject.GetComponent<Button>();
        // deleteButton = ui.transform.Find("RemoveButton").gameObject.GetComponent<Button>();

        spawnrateButton.onClick.AddListener(delegate { IncreaseSpawnRate(); });
        firerateButton.onClick.AddListener(delegate { IncreaseFireRate(); });
        rangeButton.onClick.AddListener(delegate { IncreaseRange(); });
        pathButton.onClick.AddListener(delegate { EnableDrawable(); });
        // deleteButton.onClick.AddListener(delegate { RemoveSpawner(); });

        // Fills
        GameObject infhutCanvas = ui.transform.Find("Canvas").gameObject;
        rangeFill = infhutCanvas.transform.Find("range").gameObject.GetComponent<Image>();
        fireRateFill = infhutCanvas.transform.Find("firerate").gameObject.GetComponent<Image>();
        spawnFill = infhutCanvas.transform.Find("spawn").gameObject.GetComponent<Image>();

        rangeFill.fillAmount = 0;
        fireRateFill.fillAmount = 0;
        spawnFill.fillAmount = 0;
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
            newPathPoint = AddPathMarker(spawnerTeam, point);
        }
        else
        {
            return -1;
        }

        if (spawnerTeam == "RED")
        {
            UpdateSlider(ref pathBar);
        }

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

    // TODO; RENAME TOMORROW WHEN I KNOW WHAT IM DOING
    void AddSomePoints(string color)
    {
        GameObject chosenPath = paths[Random.Range(0, paths.Count)];
        foreach (Transform orbTransform in chosenPath.transform.GetComponentsInChildren<Transform>())
        {
            if (orbTransform.position != new Vector3(0, 0.5f, 0) && orbTransform.position != new Vector3(0, 0.0f, 0))
            {
                AddPathMarker(color, orbTransform.position);
            }
        }
    }

    // TODO: This should NOT add to pathSpheres. That should be its own thing.
    public GameObject AddPathMarker(string color, Vector3 loc)
    {
        // Debug.Log("Adding path marker!" + loc);
        GameObject obj = Instantiate(pathMarker, loc, Quaternion.identity) as GameObject;
        if (color == "RED")
        {
            // TODO: This feels smart but i dont know why
            // It was :)
            obj.GetComponent<MeshRenderer>().material = redMat;
        }
        else
        {
            obj.GetComponent<MeshRenderer>().material = blueMat;
        }
        pathSpheres.Add(obj);
        return obj;
    }

    void AI_DrawPath(Vector3 position)
    {
        if (spawnerTeam == "RED")
        {
            if (paths.Count > 0)
            {
                AddSomePoints("RED");
            }
            else
            {
                GameObject obj = Instantiate(pathMarker, position, Quaternion.identity) as GameObject;
                obj.GetComponent<MeshRenderer>().material = redMat;
                pathSpheres.Add(obj);
            }
        }
        else
        {
            if (paths.Count > 0)
            {
                AddSomePoints("BLUE");
            }
            else
            {
                GameObject obj = Instantiate(pathMarker, position, Quaternion.identity) as GameObject;
                obj.GetComponent<MeshRenderer>().material = redMat;
                pathSpheres.Add(obj);
            }
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

    public void CreateNaniteGenerator()
    {
        if (SpawnerTracker.redSpawnerObjs.Count <= 2)
        {
            if (TeamStats.RedPoints >= 3)
            {
                Vector3 newPos = SpawnerTracker.redSpawnerObjs[0].transform.position;
                newPos.z += (16.0f * (SpawnerTracker.redSpawnerObjs.Count - 1.5f));
                newPos.y += 0.2f; // TODO: Why no work without? Fix later.
                GameObject newObj = Instantiate(naniteGenPrefab, newPos, Quaternion.identity) as GameObject;
                newObj.GetComponent<Structure>().type = "nanite";
                SpawnerTracker.redSpawnerObjs.Add(newObj);
                // Debug.Log("Adding nanite");

                // Costs 3 nanites.
                TeamStats.RedPoints -= 3;

                // Raise nanite production.
                TeamStats.RedNaniteGain += 1;
            }
        }
    }

    private void InstantiateUnit(GameObject reqPrefab)
    {
        // TODO: When creating, add a new field to SpawnerData to determine unit type to spawn!!!
        GameObject obj = Instantiate(reqPrefab, this.transform.position, Quaternion.identity) as GameObject;
        unitList.Add(obj);

        obj.GetComponent<Unit>().Initalize(pathSpheres, spawnerTeam, fireDelay, unitRange);
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
        yield return new WaitForSeconds(spawnTime);
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
        if (spawnTime >= 1.0f)
        {
            if (deductTeamPoints(1))
            {
                spawnTime -= 0.5f;
                spawnFill.fillAmount = calculateFill(startingSpawnTime, 0.5f, spawnTime);
            }
        }
        if (spawnTime < 1.0f)
        {
            spawnrateButton.interactable = false;
        }
    }

    public void IncreaseFireRate()
    {
        if (fireDelay >= 0.5f)
        {
            if (deductTeamPoints(1))
            {
                fireDelay -= 0.25f;
                fireRateFill.fillAmount = calculateFill(startingFireDelay, 0.25f, fireDelay);
            }
        }
        if (fireDelay < 0.5f)
        {
            firerateButton.interactable = false;
        }
    }

    public void IncreaseRange()
    {
        if (unitRange <= 6.0f)
        {
            if (deductTeamPoints(1))
            {
                unitRange += 0.5f;
                rangeFill.fillAmount = calculateFill(startingUnitRange, 6.5f, unitRange);
            }
        }
        if (unitRange > 6.0f)
        {
            rangeButton.interactable = false;
        }
    }


    public bool deductTeamPoints(int cost)
    {
        GameObject canvas = GameObject.Find("Canvas");
        if (spawnerTeam == "RED")
        {
            if (TeamStats.RedPoints >= cost)
            {
                TeamStats.RedPoints -= cost;
                return true;
            }
        }
        else
        {
            if (TeamStats.BluePoints >= cost)
            {
                TeamStats.BluePoints -= cost;
                return true;
            }
        }
        return false;
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