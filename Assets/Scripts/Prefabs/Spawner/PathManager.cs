using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PathManager : MonoBehaviour
{
    public List<GameObject> paths;
    private GameObject chosenPath;
    public GameObject pathMarker;
    // string path = "Asset_PathMarker";
    public bool pathDrawingMode = false;
    public List<GameObject> pathSpheres = new List<GameObject>();

    public Material pathMat;

    // TODO: Whenever u de-monobehaviro this
    // public PathManager(Material newMat)
    // {
    //     pathMat = newMat;
    // }

    public void SetPathMat(Material newMat)
    {
        pathMat = newMat;
    }

    void Start()
    {
        // pathMarker = Resources.Load(path) as GameObject;
    }

    public void SelectRandomPath()
    {
        chosenPath = paths[Random.Range(0, paths.Count)];
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
        if (paths.Count > 0)
        {
            PickAndCreateNewPath("BLUE");
        }
        else
        {
            GameObject obj = Instantiate(pathMarker, position, Quaternion.identity) as GameObject;
            obj.GetComponent<MeshRenderer>().material = pathMat;
            pathSpheres.Add(obj);
        }
    }

    public bool GetIsDrawable()
    {
        return pathDrawingMode;
    }

    // TODO: MAKE THIS WORK!!!
    // Returns number of pathSpheres in path now.
    // public int DrawPathSphereAtPoint(Vector3 point, ref Slider pathBar)
    // {
    //     GameObject newPathPoint;
    //     ClearNullInstances();

    //     if (pathSpheres.Count <= maxPathLength)
    //     {
    //         // Create and add a team path marker.
    //         newPathPoint = CreatePathMarker(new PathMarkerModel(spawnerTeam, point));
    //         // newPathPoint = AddPathMarkerToPathSpheres(new PathMarkerModel(spawnerTeam, point));
    //     }
    //     else
    //     {
    //         return -1;
    //     }

    //     if (spawnerTeam == "RED")
    //     {
    //         UpdateSlider(ref pathBar);
    //     }

    //     AddPathMarkerToPathSpheres(newPathPoint);
    //     AddPathPointToAlliedUnits(newPathPoint);

    //     return pathSpheres.Count;
    // }

    // TODO: Also have this only happen when EnableDrawable is pressed.
    public void ClearPoints(List<GameObject> unitList)
    {
        while (pathSpheres.Count > 0)
        {
            RemovePoint(pathSpheres[0], unitList);
        }
    }

    private void ResetPathSpheres()
    {
        pathSpheres = new List<GameObject>();
    }

    public void RemovePoint(GameObject obj, List<GameObject> unitList)
    {
        pathSpheres.Remove(obj);
        foreach (GameObject unit in unitList)
        {
            if (unit != null)
            {
                unit.GetComponent<Unit>().RemovePoint(obj);
            }
        }
        GameObject.Destroy(obj);
    }

    public void AddPathMarkerToPathSpheres(GameObject pathMarker)
    {
        pathSpheres.Add(pathMarker);
    }
}