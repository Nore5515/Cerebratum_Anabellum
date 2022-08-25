using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public GameObject Dest;
    int MoveSpeed = 4;
    double MaxDist = 1.2;
    int MinDist = 1;
    bool removing = false;

    public CubeMaker cm;

    public List<GameObject> objs = new List<GameObject>();

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
                if (Vector3.Distance(transform.position, Dest.transform.position) <= MaxDist && removing == false)
                {
                    removing = true;
                    Debug.Log("Gotem");
                    RemovePoint(Dest);
                }

            }
        }
    }

    public void AddPoint(GameObject point)
    {
        objs.Add(point);
        Debug.Log("Adding! Now has");
        Debug.Log(objs.Count);
        if (objs.Count == 1)
        {
            Dest = objs[0];
        }
        else if (objs.Count == 0)
        {
            Dest = null;
        }
        else
        {
            Dest = objs[objs.Count - 1];
        }
    }

    public void RemovePoint(GameObject point)
    {
        objs.Remove(point);
        Debug.Log("Removing! Now has");
        Debug.Log(objs.Count);
        removing = false;
        if (objs.Count == 1)
        {
            Dest = objs[0];
        }
        else if (objs.Count == 0)
        {
            Dest = null;
        }
        else
        {
            Dest = objs[objs.Count - 1];
        }
    }

}