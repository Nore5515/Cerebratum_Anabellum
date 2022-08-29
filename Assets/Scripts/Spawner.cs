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

    void Start()
    {
        IEnumerator coroutine = SpawnPrefab();
        StartCoroutine(coroutine);
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
            obj.GetComponent<Unit>().Initalize(cm.redObjs, team, fireDelay);
            obj.GetComponent<MeshRenderer>().material = redMat;
        }
        else
        {
            obj.GetComponent<Unit>().Initalize(cm.blueObjs, team, fireDelay);
            obj.GetComponent<MeshRenderer>().material = blueMat;
        }
        cm.AddUnit(obj);
        StartCoroutine(SpawnPrefab());
    }

    public void IncreaseSpawnRate()
    {
        if (spawnTime >= 1.0f)
        {
            spawnTime -= 0.5f;
        }
    }

    public void IncreaseFireRate()
    {
        if (fireDelay >= 0.5f)
        {
            fireDelay -= 0.25f;
        }
    }

}