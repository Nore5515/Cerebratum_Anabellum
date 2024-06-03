using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CustomMap : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(MapJson.Instance.mapJson);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
