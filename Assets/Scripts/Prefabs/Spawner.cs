using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnerData
{
    public string name;
    public GameObject obj;

    public void Initalize(string _name, GameObject _obj)
    {
        name = _name;
        obj = _obj;
    }
}

public class Spawner : MonoBehaviour
{
    public GameObject prefab;
    public GameObject spiderPrefab;
    public CubeMaker cm;
    public Material redMat;
    public Material blueMat;

    public List<GameObject> instances = new List<GameObject>();
    public List<GameObject> markedInstances = new List<GameObject>();
    public List<Spawner> enemySpawners = new List<Spawner>();
    public List<SpawnerData> alliedSpawnerObjs = new List<SpawnerData>();
    public List<GameObject> paths;

    public string team = "RED";
    public float fireDelay = 2.0f;
    public float unitRange = 3.0f;
    public float spawnTime = 3.0f;
    public float pointTimer = 10.0f;
    public bool isAI = false;

    // NEW STUFF
    public List<GameObject> spheres = new List<GameObject>();
    GameObject canvas;
    public GameObject pathMarker;
    string path = "Asset_PathMarker";
    public GameObject spawnerUI;

    void Start()
    {
        pathMarker = Resources.Load(path) as GameObject;
        canvas = GameObject.Find("Canvas");
        spawnerUI = this.gameObject.transform.Find("UI").gameObject;
        InitalizeUI(spawnerUI);
        
        IEnumerator coroutine = SpawnPrefab();
        StartCoroutine(coroutine);
        StartCoroutine(GainPoints());
        alliedSpawnerObjs.Add(new SpawnerData());
        alliedSpawnerObjs[0].obj = this.gameObject;
        GameObject[] spawnerObjs = GameObject.FindGameObjectsWithTag("spawner");
        foreach (var spawnerObj in spawnerObjs)
        {
            Spawner spawner = spawnerObj.GetComponent<Spawner>();
            if (spawner != null)
            {
                if (spawner.team != team)
                {
                    enemySpawners.Add(spawner);
                }
            }
        }

        // // OH THIS WORKS BC IT NEEDS TO COMPILE
        // Debug.Log(TeamStats.Kills);

        if (isAI && enemySpawners.Count >= 1)
        {
            AI_DrawPath(enemySpawners[0].gameObject.transform.position);
        }

    }

    private void InitalizeUI(GameObject ui)
    {
        Button spawnrateButton = ui.transform.Find("RSpawn").gameObject.GetComponent<Button>();
        Button firerateButton = ui.transform.Find("RFireRate").gameObject.GetComponent<Button>();
        Button rangeButton = ui.transform.Find("RRange").gameObject.GetComponent<Button>();
        Button pathButton = ui.transform.Find("RPath").gameObject.GetComponent<Button>();

        spawnrateButton.onClick.AddListener(delegate { IncreaseSpawnRate();});
        firerateButton.onClick.AddListener(delegate { IncreaseFireRate();});
        rangeButton.onClick.AddListener(delegate { IncreaseRange();});
    }

    IEnumerator GainPoints()
    {
        yield return new WaitForSeconds(pointTimer);
        
        if (team == "RED")
        {
            TeamStats.RedPoints += 1;
        }
        else
        {
            TeamStats.BluePoints += 1;
        }

        if (isAI)
        {
            AI_SpendPoints();
        }

        StartCoroutine(GainPoints());
    }
    
    // TODO; RENAME TOMORROW WHEN I KNOW WHAT IM DOING
    void AddSomePoints(string color)
    {
        GameObject chosenPath = paths[Random.Range(0, paths.Count)];
        foreach (Transform orbTransform in chosenPath.transform.GetComponentsInChildren<Transform>())
        {
            if (orbTransform.position != new Vector3(0, 0, 0))
            {
                AddPathMarker(color, orbTransform.position);
            }
        }
    }

    public GameObject AddPathMarker(string color, Vector3 loc)
    {
            GameObject obj = Instantiate(pathMarker, loc, Quaternion.identity) as GameObject;
            if (color == "RED"){
                // TODO: This feels smart but i dont know why
                // It was :)
                obj.GetComponent<MeshRenderer>().material = redMat;
            }
            else{
                obj.GetComponent<MeshRenderer>().material = blueMat;
            }
            spheres.Add(obj);
            return obj;
    }

    void AI_DrawPath(Vector3 position)
    {
        if (team == "RED")
        {
            if (paths.Count > 0)
            {
                AddSomePoints("RED");
            }
            else
            {
                GameObject obj = Instantiate(pathMarker, position, Quaternion.identity) as GameObject;
                obj.GetComponent<MeshRenderer>().material = redMat;
                spheres.Add(obj);
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
                spheres.Add(obj);
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
        foreach (GameObject instance in instances)
        {
            if (instance == null)
            {
                markedInstances.Add(instance);
            }
        }
        if (markedInstances.Count > 0)
        {
            foreach (GameObject markedInstance in markedInstances)
            {
                instances.Remove(markedInstance);
            }
            markedInstances = new List<GameObject>();
        }
    }


    public void CreateNewSpawner()
    {
        if (alliedSpawnerObjs.Count <= 2)
        {
            Vector3 newPos = alliedSpawnerObjs[0].obj.transform.position;
            newPos.z += (16.0f * (alliedSpawnerObjs.Count - 1.5f));
            GameObject newObj = Instantiate(alliedSpawnerObjs[0].obj, newPos, Quaternion.identity) as GameObject;
            SpawnerData sd = new SpawnerData();
            sd.obj = newObj;
            alliedSpawnerObjs.Add(sd);
        }
    }

    public void CreateSpidertank()
    {
        CreatePrefab(spiderPrefab);
    }

    private void CreatePrefab(GameObject reqPrefab)
    {
        foreach (SpawnerData spawnData in alliedSpawnerObjs)
        {
            GameObject obj = Instantiate(reqPrefab, spawnData.obj.transform.position, Quaternion.identity) as GameObject;
            instances.Add(obj);

            obj.GetComponent<Unit>().Initalize(spheres, team, fireDelay, unitRange);
            if (team == "RED")
            {
                obj.GetComponent<MeshRenderer>().material = redMat;
            }
            else
            {
                obj.GetComponent<MeshRenderer>().material = blueMat;
            }
        }
    }

    IEnumerator SpawnPrefab()
    {
        yield return new WaitForSeconds(spawnTime);
        ClearNullInstances();

        CreatePrefab(prefab);

        StartCoroutine(SpawnPrefab());
    }

    public void IncreaseSpawnRate()
    {
        if (spawnTime >= 1.0f)
        {
            if (deductTeamPoints(1))
            {
                spawnTime -= 0.5f;
            }
        }
    }

    public void IncreaseFireRate()
    {
        if (fireDelay >= 0.5f)
        {
            if (deductTeamPoints(1))
            {
                fireDelay -= 0.25f;
            }
        }
    }

    public void IncreaseRange()
    {
        if (unitRange <= 6.0f)
        {
            if (deductTeamPoints(1))
            {
                unitRange += 0.5f;
            }
        }
    }


    public bool deductTeamPoints(int cost)
    {
        GameObject canvas = GameObject.Find("Canvas");
        if (team == "RED")
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
        spheres.Remove(obj);
        foreach (GameObject unit in instances)
        {
            if (unit != null)
            {
                unit.GetComponent<Unit>().RemovePoint(obj);
            }
        }
        GameObject.Destroy(obj);
    }


}