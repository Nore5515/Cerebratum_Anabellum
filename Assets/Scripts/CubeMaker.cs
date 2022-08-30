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

    public List<GameObject> redObjs = new List<GameObject>();
    public List<GameObject> blueObjs = new List<GameObject>();
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
                            redObjs.Add(obj);
                        }
                        else
                        {
                            obj = Instantiate(prefabBlue, new Vector3(hit.point.x, hit.point.y, hit.point.z), Quaternion.identity) as GameObject;
                            blueObjs.Add(obj);
                        }
                        foreach (GameObject unit in units)
                        {
                            if (unit != null)
                            {
                                if (unit.GetComponent<Unit>().team == teamColor)
                                {
                                    unit.GetComponent<Unit>().AddPoint(obj);
                                }
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
                        if (redObjs.Count >= maxCount)
                        {
                            RemoveRedPoint(redObjs[0]);
                        }
                        if (blueObjs.Count >= maxCount)
                        {
                            RemoveBluePoint(blueObjs[0]);
                        }
                    }
                    if (hit.collider.gameObject.tag == "unit")
                    {
                        GameObject.Destroy(hit.collider.gameObject);
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

    public void AddRedPoint(GameObject obj)
    {
        redObjs.Add(obj);
    }
    public void AddBluePoint(GameObject obj)
    {
        blueObjs.Add(obj);
    }

    public void RemoveRedPoint(GameObject obj)
    {
        // thingy = true;
        redObjs.Remove(obj);
        foreach (GameObject unit in units)
        {
            if (unit != null)
            {
                unit.GetComponent<Unit>().RemovePoint(obj);
            }
        }
        GameObject.Destroy(obj);
    }
    public void RemoveBluePoint(GameObject obj)
    {
        // thingy = true;
        blueObjs.Remove(obj);
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
