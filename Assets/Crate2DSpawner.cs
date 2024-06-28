using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Crate2DSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject cratePrefab;

    [SerializeField]
    float respawnOdds = 0.25f;

    [SerializeField]
    float respawnOddReductionPerCrate = 0.025f;

    // Just place empty game objects in here, we parse into vectors
    public List<GameObject> preplacedCrates = new List<GameObject>();
    public List<Vector3> potentialCrateVectors = new List<Vector3>();

    // Crates we spawned already
    Dictionary<Vector3, GameObject> spawnedCrates = new Dictionary<Vector3, GameObject>();

    public void LateStart()
    {
        foreach (GameObject obj in preplacedCrates)
        {
            Vector3 pos = obj.transform.position;
            spawnedCrates.Add(pos, obj);
            potentialCrateVectors.Add(pos);
        }

        InvokeRepeating("SpawnCheck", 5.0f, 5.0f);
    }

    void Start()
    {
        LateStart();
    }

    int GetNonNullCrateCount()
    {
        int total = 0;
        foreach (Vector3 pos in spawnedCrates.Keys)
        {
            if (spawnedCrates[pos] != null)
            {
                total++;
            }
        }
        return total;
    }

    void SpawnCheck()
    {
        foreach (Vector3 cratePos in potentialCrateVectors)
        {
            if (!spawnedCrates.ContainsKey(cratePos))
            {
                spawnedCrates[cratePos] = null;
            }
            if (spawnedCrates[cratePos] == null)
            {
                if (Random.Range(0.0f, 1.0f) < (respawnOdds - (GetNonNullCrateCount() * respawnOddReductionPerCrate)))
                {
                    spawnedCrates[cratePos] = Instantiate(cratePrefab, cratePos, Quaternion.Euler(0.0f, 0.0f, 0.0f));
                }
            }
        }
    }

    public void AddNewCrateSpawn(GameObject obj)
    {
        preplacedCrates.Add(obj);
        potentialCrateVectors.Add(obj.transform.position);
        spawnedCrates[obj.transform.position] = obj;
    }
}
