using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;
    public float spawnTime = 3.0f;
    public CubeMaker cm;
    public Material redMat;
    public Material blueMat;

    public List<GameObject> instances = new List<GameObject>();
    public List<GameObject> markedInstances = new List<GameObject>();

    public string team = "RED";

    public float fireDelay = 2.0f;
    public float unitRange = 3.0f;

    public float pointTimer = 10.0f;

    void Start()
    {
        IEnumerator coroutine = SpawnPrefab();
        StartCoroutine(coroutine);
        StartCoroutine(GainPoints());
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
        StartCoroutine(GainPoints());
    }

    IEnumerator SpawnPrefab()
    {
        yield return new WaitForSeconds(spawnTime);
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

        GameObject obj = Instantiate(prefab, this.transform.position, Quaternion.identity) as GameObject;
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
        StartCoroutine(SpawnPrefab());
    }

    public void IncreaseSpawnRate()
    {
        if (deductTeamPoints())
        {
            if (spawnTime >= 1.0f)
            {
                spawnTime -= 0.5f;
            }
        }
    }

    public void IncreaseFireRate()
    {
        if (deductTeamPoints())
        {
            if (fireDelay >= 0.5f)
            {
                fireDelay -= 0.25f;
            }
        }
    }

    public void IncreaseRange()
    {
        if (deductTeamPoints())
        {
            if (unitRange <= 6.0f)
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