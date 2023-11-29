using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotentialCratePos : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnCheck());   
    }

    IEnumerator SpawnCheck()
    {
        yield return new WaitForSeconds(5f);
        //code here will execute after 5 seconds
    }
}
