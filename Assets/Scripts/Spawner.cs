using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;
    public float spawnTime = 3.0f;
    public CubeMaker cm;

    public List<GameObject> instances = new List<GameObject>();
    public List<GameObject> markedInstances = new List<GameObject>();

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
        cm.AddUnit(obj);
        StartCoroutine(SpawnPrefab());
    }

}