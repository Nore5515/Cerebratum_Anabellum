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
    public float survivalTime = 5.0f;

    public CubeMaker cm;

    public string team = "RED";

    public List<GameObject> objs = new List<GameObject>();

    void Start()
    {
        IEnumerator coroutine = SelfDestruct();
        StartCoroutine(coroutine);
    }

    public void Initalize(List<GameObject> newObjs)
    {
        foreach (GameObject obj in newObjs)
        {
            objs.Add(obj);
        }
        Dest = objs[0];
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(survivalTime);
        Destroy(gameObject);
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
                    if (Dest != null)
                    {
                        RemovePoint(Dest);
                    }
                }

            }
        }
    }

    public void AddPoint(GameObject point)
    {
        objs.Add(point);
        if (Dest == null)
        {
            Dest = objs[0];
        }
    }

    public void RemovePoint(GameObject point)
    {
        objs.Remove(point);
        removing = false;
        if (Dest != null)
        {
            if (objs.Count <= 0)
            {
                Dest = null;
            }
            else
            {
                Dest = objs[0];
            }
        }
    }

}