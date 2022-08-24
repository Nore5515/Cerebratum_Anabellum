using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    GameObject Dest;
    int MoveSpeed = 4;
    double MaxDist = 1.2;
    int MinDist = 1;

    List<GameObject> objs = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Dest != null)
        {
            transform.LookAt(Dest.transform);

            if (Vector3.Distance(transform.position, Dest.transform.position) >= MinDist)
            {
                transform.position += transform.forward * MoveSpeed * Time.deltaTime;
                if (Vector3.Distance(transform.position, Dest.transform.position) <= MaxDist)
                {
                    RemovePoint(Dest);
                    Dest = objs[0];
                }

            }
        }
    }

    public void AddPoint(GameObject point)
    {
        objs.Add(point);
        if (objs.Count == 1)
        {
            Dest = objs[0];
        }
    }

    public void RemovePoint(GameObject point)
    {
        objs.Remove(point);
    }
}

