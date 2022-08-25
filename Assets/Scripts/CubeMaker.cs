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

    List<GameObject> objs = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
    }

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
                    thingy = true;
                    GameObject obj = Instantiate(prefab, new Vector3(hit.point.x, hit.point.y, hit.point.z), Quaternion.identity) as GameObject;
                    objs.Add(obj);
                    unit.GetComponent<Unit>().AddPoint(obj);
                    if (objs.Count >= maxCount)
                    {
                        unit.GetComponent<Unit>().RemovePoint(obj);
                        // GameObject.Destroy(objs[0]);
                        // objs.RemoveAt(0);
                    }
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            thingy = false;
        }
    }

    public void annihilateObj(GameObject obj)
    {
        // unit.GetComponent<Unit>().RemovePoint(obj);
        GameObject.Destroy(objs[0]);
        objs.RemoveAt(0);
    }
}
