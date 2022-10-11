using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    void Start()
    {
        IEnumerator coroutine = SpawnPrefab();
        StartCoroutine(coroutine);
        StartCoroutine(GainPoints());
        alliedSpawnerObjs.Add(new SpawnerData());
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

        if (isAI && enemySpawners.Count >= 1)
        {
            AI_DrawPath(enemySpawners[0].gameObject.transform.position);
        }
    }

    IEnumerator GainPoints()
    {
        yield return new WaitForSeconds(pointTimer);
        GameObject canvas = GameObject.Find("Canvas");
        if (team == "RED")
        {
            canvas.GetComponent<UI>().ChangePoints(1, 0);
        }
        else
        {
            canvas.GetComponent<UI>().ChangePoints(0, 1);
        }
        if (isAI)
        {
            AI_SpendPoints();
        }
        StartCoroutine(GainPoints());
    }

    void AI_DrawPath(Vector3 position)
    {
        if (team == "RED")
        {
            if (paths.Count > 0)
            {
                GameObject chosenPath = paths[Random.Range(0, paths.Count)];
                foreach (Transform orbTransform in chosenPath.transform.GetComponentsInChildren<Transform>())
                {
                    if (orbTransform.position != new Vector3(0, 0, 0))
                    {
                        cm.AddRedPoint(cm.CreateRedPoint(orbTransform.position));
                    }
                }
            }
            else
            {
                cm.AddRedPoint(cm.CreateRedPoint(position));
            }
        }
        else
        {
            if (paths.Count > 0)
            {
                GameObject chosenPath = paths[Random.Range(0, paths.Count)];
                foreach (Transform orbTransform in chosenPath.transform.GetComponentsInChildren<Transform>())
                {
                    if (orbTransform.position != new Vector3(0, 0, 0))
                    {
                        cm.AddBluePoint(cm.CreateBluePoint(orbTransform.position));
                    }
                }
            }
            else
            {
                cm.AddBluePoint(cm.CreateBluePoint(position));
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
            Vector3 newPos = alliedSpawnerObjs[0].transform.position;
            newPos.z += (16.0f * (alliedSpawnerObjs.Count - 1.5f));
            GameObject newObj = Instantiate(alliedSpawnerObjs[0], newPos, Quaternion.identity) as GameObject;
            alliedSpawnerObjs.Add(newObj);
        }
    }

    IEnumerator SpawnPrefab()
    {
        yield return new WaitForSeconds(spawnTime);
        ClearNullInstances();

        foreach (GameObject spawnerObj in alliedSpawnerObjs)
        {
            GameObject obj = Instantiate(prefab, spawnerObj.transform.position, Quaternion.identity) as GameObject;
            instances.Add(obj);

            if (team == "RED")
            {
                obj.GetComponent<Unit>().Initalize(cm.redObjs, team, fireDelay, unitRange);
                obj.GetComponent<MeshRenderer>().material = redMat;
            }
            else
            {
                obj.GetComponent<Unit>().Initalize(cm.blueObjs, team, fireDelay, unitRange);
                obj.GetComponent<MeshRenderer>().material = blueMat;
            }
            cm.AddUnit(obj);
        }

        StartCoroutine(SpawnPrefab());
    }

    public void IncreaseSpawnRate()
    {
        if (spawnTime >= 1.0f)
        {
            if (deductTeamPoints())
            {
                spawnTime -= 0.5f;
            }
        }
    }

    public void IncreaseFireRate()
    {
        if (fireDelay >= 0.5f)
        {
            if (deductTeamPoints())
            {
                fireDelay -= 0.25f;
            }
        }
    }

    public void IncreaseRange()
    {
        if (unitRange <= 6.0f)
        {
            if (deductTeamPoints())
            {
                unitRange += 0.5f;
            }
        }
    }


    public bool deductTeamPoints()
    {
        GameObject canvas = GameObject.Find("Canvas");
        if (team == "RED")
        {
            if (canvas.GetComponent<UI>().redPoints > 0)
            {
                canvas.GetComponent<UI>().ChangePoints(-1, 0);
                return true;
            }
        }
        else
        {
            if (canvas.GetComponent<UI>().bluePoints > 0)
            {
                canvas.GetComponent<UI>().ChangePoints(0, -1);
                return true;
            }
        }
        return false;
    }


}