using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

// New: What is state? Also, this below text is just wrong. 
// All this does is handle paths from user input to state.
public class SpawnerPathManager : MonoBehaviour
{
    public List<GameObject> paths;
    private GameObject chosenPath;
    public GameObject pathMarker;
    public bool pathDrawingMode = false;
    public List<GameObject> pathSpheres = new List<GameObject>();

    public Material pathMat;

    bool aiControlled = false;
    int waiter = 0;

    public void Start()
    {
        PathState.paths = paths;
        if (this.gameObject.GetComponent<Spawner>() != null)
        {
            // TODO: Obviously not good long temr lol
            if (this.gameObject.GetComponent<Spawner>().spawnerTeam == "BLUE")
            {
                Debug.Log("Blue!");
                aiControlled = true;
            }
        }

        foreach (GameObject pathParent in GameObject.FindGameObjectsWithTag("path"))
        {
            paths.Add(pathParent);
        }
    }

    private void Update()
    {
        if (aiControlled)
        {
            if (paths.Count == 0 && pathSpheres.Count <= 1)
            {
                if (waiter > 300)
                {
                    List<GameObject> hqs = new List<GameObject>(GameObject.FindGameObjectsWithTag("hq"));
                    Debug.Log(hqs.Count);
                    waiter = 0;
                    foreach (var spawner in hqs)
                    {
                        if (spawner.GetComponent<HQObject>() != null)
                        {
                            if (spawner.GetComponent<HQObject>().team != "BLUE")
                            {
                                Debug.Log("GOOO HQ");
                                GameObject marker = InstantiateBluePathMarkerAtPoint(spawner.gameObject.transform.position);
                                AddPathMarkerToPathSpheres(marker);
                                return;
                            }
                        }
                    }

                    List<GameObject> spawners = new List<GameObject>(GameObject.FindGameObjectsWithTag("spawner"));
                    Debug.Log(spawners.Count);
                    waiter = 0;
                    foreach (var spawner in spawners)
                    {
                        if (spawner.GetComponent<Spawner>() != null)
                        {
                            if (spawner.GetComponent<Spawner>().spawnerTeam != "BLUE")
                            {
                                Debug.Log("GOOO");
                                GameObject marker = InstantiateBluePathMarkerAtPoint(spawner.gameObject.transform.position);
                                AddPathMarkerToPathSpheres(marker);
                                return;
                            }
                        }
                    }
                }
                else
                {
                    waiter++;
                }
            }
            else if (paths.Count > 0 && pathSpheres.Count == 0)
            {
                Debug.Log("PATHS?");
                int randomlySelectedPath = Random.Range(0, paths.Count);
                foreach (Transform child in paths[randomlySelectedPath].transform)
                {
                    GameObject marker = InstantiateBluePathMarkerAtPoint(child.position);
                    AddPathMarkerToPathSpheres(marker);
                    return;
                }
            }
        }
    }

    public void SetPathMat(Material newMat)
    {
        pathMat = newMat;
    }

    public void SelectRandomPath()
    {
        chosenPath = PathState.paths[Random.Range(0, PathState.paths.Count)];
    }

    public void PickAndCreateNewPath(string color)
    {
        ResetPathSpheres();
        SelectRandomPath();
        foreach (Transform orbTransform in chosenPath.transform.GetComponentsInChildren<Transform>())
        {
            if (orbTransform.position != new Vector3(0, 0.5f, 0) && orbTransform.position != new Vector3(0, 0.0f, 0))
            {
                GameObject pathMarker = CreatePathMarker(new PathMarkerModel(color, orbTransform.position));
                AddPathMarkerToPathSpheres(pathMarker);
            }
        }
    }

    public GameObject CreatePathMarker(PathMarkerModel pathMarkerValues)
    {
        if (pathMarkerValues.color == "RED")
        {
            return InstantiateRedPathMarkerAtPoint(pathMarkerValues.pathMarkerLoc);
        }
        else
        {
            return InstantiateBluePathMarkerAtPoint(pathMarkerValues.pathMarkerLoc);
        }
    }

    private GameObject InstantiateRedPathMarkerAtPoint(Vector3 pathMarkerPoint)
    {
        GameObject redPathMarker = Instantiate(pathMarker, pathMarkerPoint, Quaternion.identity) as GameObject;
        redPathMarker.GetComponent<MeshRenderer>().material = pathMat;
        return redPathMarker;
    }

    private GameObject InstantiateBluePathMarkerAtPoint(Vector3 pathMarkerPoint)
    {
        GameObject bluePathMarker = Instantiate(pathMarker, pathMarkerPoint, Quaternion.identity) as GameObject;
        bluePathMarker.GetComponent<MeshRenderer>().material = pathMat;
        bluePathMarker.GetComponent<MeshRenderer>().enabled = false;
        return bluePathMarker;
    }

    public void AI_DrawPath(Vector3 position)
    {
        if (PathState.paths.Count > 0)
        {
            PickAndCreateNewPath("BLUE");
        }
        else
        {
            GameObject obj = Instantiate(pathMarker, position, Quaternion.identity) as GameObject;
            obj.GetComponent<MeshRenderer>().material = pathMat;
            obj.GetComponent<MeshRenderer>().enabled = false;
            pathSpheres.Add(obj);
        }
    }

    public bool GetIsDrawable()
    {
        return pathDrawingMode;
    }

    // TODO: Also have this only happen when EnableDrawable is pressed.
    public void ClearPoints(List<GameObject> unitList)
    {
        while (pathSpheres.Count > 0)
        {
            RemovePoint(pathSpheres[0], UnitList.unitList);
        }
    }

    public void UpdatePathlessUnits(List<GameObject> unitList)
    {
        foreach (GameObject unit in unitList)
        {
            if (unit != null)
            {
                if (!unit.GetComponent<Unit>().isUnitInitialized)
                {
                    UpdateUnitWithPath(unit.GetComponent<Unit>());
                    unit.GetComponent<Unit>().isUnitInitialized = true;
                }
            }
        }
    }

    private void UpdateUnitWithPath(Unit unit)
    {
        foreach (GameObject pathSphere in pathSpheres)
        {
            unit.AddPoint(pathSphere.transform.position);
        }
    }

    private void ResetPathSpheres()
    {
        pathSpheres = new List<GameObject>();
    }

    public void RemovePoint(GameObject obj, List<GameObject> unitList)
    {
        pathSpheres.Remove(obj);
        Destroy(obj);
    }

    public void AddPathMarkerToPathSpheres(GameObject pathMarker)
    {
        pathSpheres.Add(pathMarker);
    }

    public List<Vector3> GetPathPoints()
    {
        List<Vector3> points = new List<Vector3>();
        foreach (GameObject pathSphere in pathSpheres)
        {
            points.Add(pathSphere.transform.position);
        }
        return points;
    }
}