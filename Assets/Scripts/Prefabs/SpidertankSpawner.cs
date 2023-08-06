using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpidertankSpawner : MonoBehaviour
{

    public GameObject spidertankPrefab;

    public void CreateSpidertank()
    {
        GameObject newObj = Instantiate(spidertankPrefab, this.transform.position, Quaternion.identity) as GameObject;
    }
}
