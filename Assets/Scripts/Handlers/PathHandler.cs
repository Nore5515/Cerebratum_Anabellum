using UnityEngine;
using UnityEngine.UI;

public class PathHandler : MonoBehaviour
{
    public bool pathDrawingMode = false;

    public float distancePerSphere = 0.0f;
    public float maxDistancePerSphere = 5.0f;
    public Vector3 oldPos = new Vector3();

    public int xGridOffset;
    public int yGridOffset;

    Color green = new Color(88f / 255f, 233f / 255f, 55f / 255f);

    // NEW STUFF
    //public Slider pathBar;

    public GameObject spawnerSource;

    public void DeselectSpawners()
    {
        if (spawnerSource == null) return;
        Spawner spawnerClass = spawnerSource.GetComponent<Spawner>();
        if (spawnerClass == null) return;
        StopDrawingPath();
        spawnerClass.SetUIVisible(false);
        HideAllSpawnerPoints(spawnerClass);
    }

    public void HandleClickOnDrawButton(RayObj rayObj)
    {
        if (IsHitObjectSelectedSpawner(rayObj))
        {
            HandleDrawButtonPress();
        }
    }

    void HandleDrawButtonPress()
    {
        Spawner spawnerClass = spawnerSource.GetComponent<Spawner>();
        if (spawnerClass == null) return;
        if (spawnerClass.spawnerTeam == "NEUTRAL") return;
        spawnerClass.EnableDrawable();
        UpdateLocalPathDrawingMode(spawnerClass);
    }

    public void HandleClickOnSpawner(RayObj rayObj)
    {
        if (IsHitObjectNonAlliedSpawner(rayObj)) return;

        if (IsHitObjectSelectedSpawner(rayObj))
        {
            HandleClickOnSelectedSpawner();
            return;
        }

        // Spawner isn't selected, select it
        DeselectSpawners();
        SelectSpawner(rayObj.hit.collider.gameObject);
    }

    bool IsHitObjectNonAlliedSpawner(RayObj rayObj)
    {
        if (rayObj.hit.collider.transform.GetComponent<NeutralSpawner>() != null)
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
        UpdateLocalPathDrawingMode(spawnerClass);
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


    void UpdateLocalPathDrawingMode(Spawner spawnerClass)
    {
        pathDrawingMode = spawnerClass.spawnerPathManager.GetIsDrawable();
    }

    public bool IsHitObjectSelectedSpawner(RayObj rayObj)
    {
        GameObject spawnerObj = GetSpawnerObjFromRayObj(rayObj);

        return (spawnerSource == spawnerObj);
    }

    GameObject GetSpawnerObjFromRayObj(RayObj rayObj)
    {
        if (rayObj.hit.collider.name == "DrawCube")
        {
            return rayObj.hit.collider.transform.parent.gameObject;
        }
        else
        {
            return rayObj.hit.collider.gameObject;
        }
    }

    void PlaceFollowObjOnPosition(Vector3 position)
    {
        if (distancePerSphere >= maxDistancePerSphere)
        {
            PlaceFollowSphere(position);
        }
        // If distance is less than maxdistancepersphere, add change in distance.
        else
        {
            AddSphereDistance(position);
        }
    }

    void PlaceFollowSphere(Vector3 vector)
    {
        if (spawnerSource == null) { Debug.Log("Spawner is null"); return; }

        Spawner spawnerClass = spawnerSource.GetComponent<Spawner>();

        ResetSphereDistance();
        spawnerClass.DrawPathSphereAtPoint(vector);
    }

    void ResetSphereDistance()
    {
        distancePerSphere = 0.0f;
    }

    void AddSphereDistance(Vector3 vector)
    {
        if (oldPos == new Vector3(0.0f, 0.0f, 0.0f))
        {
            oldPos = vector;
        }
        else
        {
            distancePerSphere += Vector3.Distance(oldPos, vector);
            oldPos = vector;
        }
    }

    void PreparePathBar()
    {
        //pathBar.value = 0;
        //pathBar.gameObject.SetActive(true);
        //GetImageFromPathBarObj(pathBar.gameObject).color = green;
    }

    Image GetImageFromPathBarObj(GameObject pathBarObj)
    {
        return pathBarObj.transform.Find("Fill Area").Find("Fill").GetComponent<Image>();
    }

    public void StopDrawingPath()
    {
        pathDrawingMode = false;
        //pathBar.gameObject.SetActive(false);
        if (spawnerSource != null)
        {
            if (spawnerSource.GetComponent<Spawner>() != null)
            {
                spawnerSource.GetComponent<Spawner>().SetIsDrawable(false);
                spawnerSource.GetComponent<Spawner>().UpdateAwaitingUnits();
            }
        }
    }

    public void MouseHeldAndDraggedAtPosition(Vector3 mousePosition)
    {
        PlaceFollowObjOnPosition(mousePosition);
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
        foreach (GameObject point in spawner.spawnerPathManager.pathSpheres)
        {
            point.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
