using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerLoader : MonoBehaviour
{
    GameObject[] spawners;
    [SerializeField]
    bool preset;

    public void LateStart()
    {
        spawners = GameObject.FindGameObjectsWithTag("spawner");
        ConfigureSpawnersFromConfig();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (preset)
        {
            LateStart();
        }
    }

    void ConfigureSpawnersFromConfig()
    {
        Dictionary<string, int> teamSpawners = new Dictionary<string, int>();

        foreach (GameObject spawner in spawners)
        {
            Spawner spawnerClass = spawner.GetComponent<Spawner>();
            if (spawnerClass == null) { continue; }

            if (!teamSpawners.ContainsKey(spawnerClass.spawnerTeam))
            {
                teamSpawners.Add(spawnerClass.spawnerTeam, 0);
            }

            // TODO: Relic of old way
            if (teamSpawners[spawnerClass.spawnerTeam] < Constants.SPAWNERS_PER_SIDE || true)
            {
                spawner.SetActive(true);
                teamSpawners[spawnerClass.spawnerTeam]++;
            }
            else
            {
                spawner.SetActive(false);
            }
        }
    }
}
