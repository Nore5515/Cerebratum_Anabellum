using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CubeMaker : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;
    public GameObject prefabRed;
    public GameObject prefabBlue;
    public int maxCount;
    bool thingy;

    public string teamColor = "RED";
    public Text teamColorText;

    public List<GameObject> objs = new List<GameObject>();
    public List<GameObject> units = new List<GameObject>();
    public List<GameObject> toRemoveUnits = new List<GameObject>();


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            teamColor = "RED";
            teamColorText.text = teamColor;
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            teamColor = "BLUE";
            teamColorText.text = teamColor;
        }
        if (thingy == false)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    if (hit.collider.gameObject.tag == "floor")
                    {
                        GameObject obj;
                        if (teamColor == "RED")
                        {
                            obj = Instantiate(prefabRed, new Vector3(hit.point.x, hit.point.y, hit.point.z), Quaternion.identity) as GameObject;
                        }
                        else
                        {
                            obj = Instantiate(prefabBlue, new Vector3(hit.point.x, hit.point.y, hit.point.z), Quaternion.identity) as GameObject;
                        }
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
        }
        // if (Input.GetKeyUp(KeyCode.Mouse0))
        // {
        // thingy = false;
        // }
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
        // thingy = true;
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
