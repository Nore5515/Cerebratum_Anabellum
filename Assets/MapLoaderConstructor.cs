using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class MapLoaderConstructor : MonoBehaviour
{

    [SerializeField]
    GameObject hqPrefab;

    [SerializeField]
    GameObject spawnerPrefab;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlaceHQAtLocation(Vector3 location, string team)
    {
        GameObject obj = Instantiate(hqPrefab);
        obj.GetComponent<HQObject>().team = team;
        StageUtility.PlaceGameObjectInCurrentStage(obj);
        obj.transform.position = location;
    }

    public void PlaceSpawnerAtLocation(Vector3 location, string team)
    {
        GameObject obj = Instantiate(spawnerPrefab);
        Spawner spawn = obj.GetComponent<Spawner>();
        spawn.unitType = Constants.INF_TYPE;
        spawn.spawnerTeam = team;
        spawn.LateStart();
        StageUtility.PlaceGameObjectInCurrentStage(obj);
        obj.transform.position = location;
    }


}
