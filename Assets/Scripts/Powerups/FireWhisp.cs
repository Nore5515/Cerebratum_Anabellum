using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWhisp : MonoBehaviour
{
    [SerializeField]
    float survivalTime;

    // Start is called before the first frame update
    void Start()
    {
        IEnumerator coroutine = SelfDestruct();
        StartCoroutine(coroutine);
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(survivalTime);
        Destroy(gameObject);
    }
}
