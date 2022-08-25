using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMaker : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;
    public GameObject prefab;
    public int maxCount;
    bool thingy;

    public List<GameObject> objs = new List<GameObject>();
    public List<GameObject> units = new List<GameObject>();
    public List<GameObject> toRemoveUnits = new List<GameObject>();


    // Update is called once per frame
    void Update()
    {
        if (thingy == false)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    GameObject obj = Instantiate(prefab, new Vector3(hit.point.x, hit.point.y, hit.point.z), Quaternion.identity) as GameObject;
                    objs.Add(obj);
                    foreach (GameObject unit in units)
                    {
                        if (unit != null)
                        {
                            unit.GetComponent<Unit>().AddPoint(obj);
                        }
                        else
                        {
                            toRemoveUnits.Add(unit);
                        }
                    }
                    foreach (GameObject markedUnit in toRemoveUnits)
                    {
                        units.Remove(markedUnit);
                    }
                    toRemoveUnits = new List<GameObject>();
                    if (objs.Count >= maxCount)
                    {
                        RemovePoint(objs[0]);
                    }
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            thingy = false;
        }
    }

    public void AddUnit(GameObject newUnit)
    {
        units.Add(newUnit);
    }

    public void AddPoint(GameObject obj)
    {
        objs.Add(obj);
    }

    public void RemovePoint(GameObject obj)
    {
        thingy = true;
        objs.Remove(obj);
        foreach (GameObject unit in units)
        {
            if (unit != null)
            {
                unit.GetComponent<Unit>().RemovePoint(obj);
            }
        }
        GameObject.Destroy(obj);
    }

}
