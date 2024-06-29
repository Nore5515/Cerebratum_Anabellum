using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoaderConstructor : MonoBehaviour
{

    [SerializeField]
    GameObject hqPrefab;

    [SerializeField]
    GameObject spawnerPrefab;

    [SerializeField]
    GameObject cratePrefab;

    [SerializeField]
    Crate2DSpawner crateSpawner;

    public void PlaceHQAtLocation(Vector3 location, string team)
    {
        GameObject obj = Instantiate(hqPrefab);
        obj.GetComponent<HQObject>().team = team;
        //StageUtility.PlaceGameObjectInCurrentStage(obj);
        obj.transform.position = location;
    }

    public void PlaceSpawnerAtLocation(Vector3 location, string team)
    {
        GameObject obj = Instantiate(spawnerPrefab);
        Spawner spawn = obj.GetComponent<Spawner>();
        spawn.unitType = Constants.INF_TYPE;
        spawn.spawnerTeam = team;
        spawn.LateStart();
        //StageUtility.PlaceGameObjectInCurrentStage(obj);
        obj.transform.position = location;
    }

    public void PlaceCrateAtLocation(Vector3 location)
    {
        GameObject obj = Instantiate(cratePrefab);
        Crate2D crate = obj.GetComponent<Crate2D>();
        crate.LateStart();
        //StageUtility.PlaceGameObjectInCurrentStage(obj);
        obj.transform.position = location;
        AddNewCrateToSpawner(obj);
    }

    void AddNewCrateToSpawner(GameObject obj)
    {
        crateSpawner.AddNewCrateSpawn(obj);
    }
}
