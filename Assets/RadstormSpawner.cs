using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadstormSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject radstormPrefab;

    [SerializeField]
    List<GameObject> radstormDestinations;

    [SerializeField]
    float gracePeriod = 0.0f;

    [SerializeField]
    float tickSpeed = 1.0f;

    [SerializeField]
    float chanceOfSpawn = 1.0f;

    [SerializeField]
    Vector3 stormSize;

    [SerializeField]
    float stormSpeed;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnTick", gracePeriod, tickSpeed);  //0.25 delay, repeat every 0.25s
    }

    void SpawnTick()
    {
        if (Random.Range(0.0f, 1.0f) <= chanceOfSpawn && radstormDestinations.Count > 0)
        {
            GameObject instance = Instantiate(radstormPrefab, transform.position, transform.rotation);
            instance.GetComponent<Radstorm>().BeginJourney(GetRandomDestination());
            instance.transform.localScale = stormSize;
        }
    }

    Vector3 GetRandomDestination()
    {
        if (radstormDestinations.Count > 0)
        {
            int randDest = Random.Range(0, radstormDestinations.Count);
            return radstormDestinations[randDest].transform.position;
        }
        return new Vector3(0.0f, 0.0f, 0.0f);
    }
}
