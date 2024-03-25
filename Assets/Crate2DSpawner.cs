using System.Collections.Generic;
using UnityEngine;

public class Crate2DSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject cratePrefab;

    [SerializeField]
    float respawnOdds = 0.25f;

    [SerializeField]
    float respawnOddReductionPerCrate = 0.025f;

    // Just place empty game objects in here
    public List<GameObject> potentialCratePositions = new List<GameObject>();

    // Crates we spawned already
    Dictionary<GameObject, GameObject> spawnedCrates = new Dictionary<GameObject, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject obj in potentialCratePositions)
        {
            spawnedCrates.Add(obj, null);
        }

        InvokeRepeating("SpawnCheck", 5.0f, 5.0f);
    }

    int GetNonNullCrateCount()
    {
        int total = 0;
        foreach (GameObject pos in spawnedCrates.Keys)
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
        foreach (GameObject pos in potentialCratePositions)
        {
            if (spawnedCrates[pos] == null)
            {
                if (Random.Range(0.0f, 1.0f) < (respawnOdds - (GetNonNullCrateCount() * respawnOddReductionPerCrate)))
                {
                    spawnedCrates[pos] = Instantiate(cratePrefab, pos.transform.position, pos.transform.rotation);
                }
            }
        }
    }
}
