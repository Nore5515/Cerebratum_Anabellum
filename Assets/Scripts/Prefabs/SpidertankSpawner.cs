using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpidertankSpawner : MonoBehaviour
{

    public GameObject spidertankPrefab;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateSpidertank()
    {
        GameObject newObj = Instantiate(spidertankPrefab, this.transform.position, Quaternion.identity) as GameObject;
    }
}
