using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMaker : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;
    public GameObject prefab;
    public GameObject unit;
    public int maxCount;
    bool thingy;

    public List<GameObject> objs = new List<GameObject>();

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
                    unit.GetComponent<Unit>().AddPoint(obj);
                    if (objs.Count >= maxCount)
                    {
                        RemovePoint(objs[0]);
                        //unit.GetComponent<Unit>().RemovePoint(obj);
                    }
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            thingy = false;
        }
    }

    public void AddPoint(GameObject obj)
    {
        objs.Add(obj);
    }

    public void RemovePoint(GameObject obj)
    {
        thingy = true;
        objs.Remove(obj);
        GameObject.Destroy(obj);
    }

}
