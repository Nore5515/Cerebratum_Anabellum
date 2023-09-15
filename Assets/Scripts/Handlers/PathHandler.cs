using UnityEngine;
using UnityEngine.UI;

public class PathHandler : MonoBehaviour
{
    public GameObject prefabRed;
    public GameObject prefabBlue;

    public bool pathDrawingMode = false;

    public float distancePerSphere = 0.0f;
    public float maxDistancePerSphere = 5.0f;
    public Vector3 oldPos = new Vector3();

    Color green = new Color(88f / 255f, 233f / 255f, 55f / 255f);

    // NEW STUFF
    public Slider pathBar;

    GameObject spawnerSource;

    public PosHandler pathHandlerPossessionHandler;

    public void Start()
    {
        GameObject.Find("Canvas/UnitStatsWBG").SetActive(false);
    }

    public void SetPathDrawingMode(bool newMode)
    {
        pathDrawingMode = newMode;
    }

    public void DeselectSpawners()
    {
        if (spawnerSource == null) return;
        Spawner spawnerClass = spawnerSource.GetComponent<Spawner>();
        StopDrawingPath();
        spawnerClass.SetUIVisible(false);
        HideAllSpawnerPoints(spawnerClass);
    }

    public void HandleClickOnSpawner(RayObj rayObj)
    {
        if (IsHitObjectNonAlliedSpawner(rayObj)) return;
        if (IsHitObjectSelectedSpawner(rayObj))
        {
            HandleClickOnSelectedSpawner();
        }
        else
        {
            DeselectSpawners();
            SelectSpawner(rayObj.hit.collider.gameObject);
        }
    }

    bool IsHitObjectNonAlliedSpawner(RayObj rayObj)
    {
        if (rayObj.hit.collider.transform.parent.gameObject.GetComponent<NeutralSpawner>() != null)
        {
            return true;
        }
        return false;
    }

    public void HandleClickOnSelectedSpawner()
    {
        Spawner spawnerClass = spawnerSource.GetComponent<Spawner>();
        if (spawnerClass == null) return;
        if (spawnerClass.spawnerTeam == "NEUTRAL") return;
        UpdatePathDrawingMode(spawnerClass);
        if (pathDrawingMode)
        {
            PreparePathBar();
        }
        else
        {
            DeselectSpawners();
            spawnerSource = null;
        }
    }

    void UpdatePathDrawingMode(Spawner spawnerClass)
    {
        pathDrawingMode = spawnerClass.spawnerPathManager.GetIsDrawable();
    }

    public bool IsHitObjectSelectedSpawner(RayObj rayObj)
    {
        return (spawnerSource == rayObj.hit.collider.gameObject);
    }

    // Attempt to place a sphere down on where the raycast hits the world.
    void TryPlaceFollowSphere(RayObj rayObj)
    {
        if (distancePerSphere >= maxDistancePerSphere)
        {
            PlaceFollowSphere(rayObj);
        }

        // If distance is less than maxdistancepersphere, add change in distance.
        else
        {
            AddSphereDistance(rayObj);
        }
    }

    void PlaceFollowSphere(RayObj rayObj)
    {
        Spawner spawnerClass = spawnerSource.GetComponent<Spawner>();

        if (DidRayObjHitBarrier(rayObj))
        {
            RayObjHitBarrier(spawnerClass);
            return;
        }

        distancePerSphere = 0.0f;
        spawnerClass.DrawPathSphereAtPoint(rayObj.hit.point, ref pathBar);
    }

    bool DidRayObjHitBarrier(RayObj rayObj)
    {
        if (rayObj.hit.collider.gameObject.CompareTag("barrier"))
        {
            return true;
        }
        return false;
    }

    void RayObjHitBarrier(Spawner spawnerClass)
    {
        spawnerClass.DisableDrawable();
        StopDrawingPath();
    }

    void AddSphereDistance(RayObj rayObj)
    {
        if (oldPos == new Vector3(0.0f, 0.0f, 0.0f))
        {
            oldPos = rayObj.hit.collider.gameObject.transform.position;
        }
        else
        {
            distancePerSphere += Vector3.Distance(oldPos, rayObj.hit.point);
            oldPos = rayObj.hit.point;
        }
    }

    void PreparePathBar()
    {
        pathBar.value = 0;
        pathBar.gameObject.SetActive(true);
        GetImageFromPathBarObj(pathBar.gameObject).color = green;
    }

    Image GetImageFromPathBarObj(GameObject pathBarObj)
    {
        return pathBarObj.transform.Find("Fill Area").Find("Fill").GetComponent<Image>();
    }

    public void StopDrawingPath()
    {
        pathDrawingMode = false;
        pathBar.gameObject.SetActive(false);
        if (spawnerSource != null)
        {
            if (spawnerSource.GetComponent<Spawner>() != null)
            {
                spawnerSource.GetComponent<Spawner>().SetIsDrawable(false);
                spawnerSource.GetComponent<Spawner>().UpdateAwaitingUnits();
            }
        }
    }

    public void AttemptPlaceSpawnerFollowObj(RayObj rayObj)
    {
        if (spawnerSource == null) return;
        if (!pathDrawingMode) return;
        if (rayObj.hit.collider == null) return;
        if (rayObj.hit.collider.gameObject.tag == "floor" || rayObj.hit.collider.gameObject.tag == "barrier")
        {
            TryPlaceFollowSphere(rayObj);
        }
    }

    private int GenerateRayFromMasks(int[] masks)
    {
        int layerMask = 1;
        foreach (int mask in masks)
        {
            layerMask = (1 << mask);
        }
        return layerMask;
    }

    void SelectSpawner(GameObject spawnerGameObject)
    {
        spawnerSource = spawnerGameObject;
        Spawner spawnerClass = spawnerSource.GetComponent<Spawner>();

        if (spawnerClass == null) return;

        HideAllSpawnerPoints(spawnerClass);
        spawnerClass.SetUIVisible(true);
        ShowAllSpawnerPoints(spawnerClass);
    }

    void ShowAllSpawnerPoints(Spawner spawner)
    {
        foreach (GameObject point in spawner.spawnerPathManager.pathSpheres)
        {
            point.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    void HideAllSpawnerPoints(Spawner spawner)
    {
        foreach(GameObject point in spawner.spawnerPathManager.pathSpheres)
        {
            point.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    void Update()
    {
        pathHandlerPossessionHandler.GetPossessionMovement();
    }

    //public GameObject CreateRedPoint(Vector3 position)
    //{
    //    return Instantiate(prefabRed, position, Quaternion.identity);
    //}

    //public GameObject CreateBluePoint(Vector3 position)
    //{
    //    return Instantiate(prefabBlue, position, Quaternion.identity);
    //}

}
