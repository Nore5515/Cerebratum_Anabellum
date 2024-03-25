using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCommander : MonoBehaviour
{
    public List<GameObject> mySpawners = new List<GameObject>();

    public string myTeam = Constants.BLUE_TEAM;


    // Start is called before the first frame update
    void Start()
    {
        PopulateSpawners();
        InvokeRepeating("EnemyLogicTick", 5.0f, 5.0f);
    }

    void PopulateSpawners()
    {
        GameObject[] allSpawners = GameObject.FindGameObjectsWithTag("spawner");
        foreach (GameObject spawner in allSpawners)
        {
            if (spawner.GetComponent<Spawner>() != null)
            {
                if (spawner.GetComponent<Spawner>().spawnerTeam == myTeam)
                {
                    mySpawners.Add(spawner);
                }
            }
        }
    }

    void EnemyLogicTick()
    {
        string enemyThoughts = "";
        enemyThoughts += "Enemy Brain Tick!";
        foreach (GameObject spawner in mySpawners)
        {
            spawner.GetComponent<Spawner>().SpawnScout();
            enemyThoughts += "\n\tAttempting to spawn a scout!";
        }
        Debug.Log(enemyThoughts);
    }
}
