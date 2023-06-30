using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PathHandler : MonoBehaviour
{
    public GameObject prefabRed;
    public GameObject prefabBlue;

    public bool pathDrawingMode = false;


    public Text teamColorText;

    // public List<GameObject> toRemoveUnits = new List<GameObject>();


    public Vector3 unitDirection = new Vector3();


    public float distancePerSphere = 0.0f;
    public float maxDistancePerSphere = 5.0f;
    public Vector3 oldPos = new Vector3();

    // NEW STUFF
    Color red = new Color(233f / 255f, 80f / 255f, 55f / 255f);
    public Slider pathBar;
    private Image pathBarFill;

    GameObject spawnerSource;

    public PossessionHandler pathHandlerPossessionHandler;

    public void Start()
    {
        pathBarFill = pathBar.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>();

        PossessionHandler.setUnitStatUI(GameObject.Find("Canvas/UnitStats"));
        GameObject.Find("Canvas/UnitStats").SetActive(false);
    }

    public void SetPathDrawingMode(bool newMode)
    {
        // pathDrawingMode = newMode;
    }


    public void DeselectSpawners()
    {
        if (spawnerSource != null)
        {
            Spawner spawnerClass = spawnerSource.GetComponent<Spawner>();
            StopDrawingPath();
            spawnerClass.SetUIVisible(false);
        }
    }

    public void HandleClickOnSpawner(RayObj rayObj)
    {
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

    public void HandleClickOnSelectedSpawner()
    {
        Spawner spawnerClass = spawnerSource.GetComponent<Spawner>();
        if (spawnerClass.spawnerPathManager.GetIsDrawable() == true)
        {
            pathDrawingMode = spawnerClass.spawnerPathManager.GetIsDrawable();
            PreparePathBar();
        }
        else
        {
            DeselectSpawners();
            spawnerSource = null;
        }
    }

    public bool IsHitObjectSelectedSpawner(RayObj rayObj)
    {
        return (spawnerSource == rayObj.hit.collider.gameObject);
    }


    // void KeyChecks()
    // {
    //     if (Input.GetKey(KeyCode.Escape))
    //     {
    //         SceneManager.LoadScene("MainMenu");
    //     }
    //     if (Input.GetKey(KeyCode.LeftControl))
    //     {
    //         SetPossession(true);
    //     }
    //     if (Input.GetKeyUp(KeyCode.Space))
    //     {
    //         FreePossession();
    //     }
    // }

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

        distancePerSphere = 0.0f;

        spawnerClass.DrawPathSphereAtPoint(rayObj.hit.point, ref pathBar);
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
        Color green = new Color(88f / 255f, 233f / 255f, 55f / 255f);
        pathBar.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = green;
    }

    public void StopDrawingPath()
    {
        // pathDrawingMode = false;
        pathBar.gameObject.SetActive(false);
        if (spawnerSource != null)
        {
            spawnerSource.GetComponent<Spawner>().SetIsDrawable(false);
        }
    }

    public void AttemptFollowSphere(RayObj rayObj)
    {
        if (spawnerSource == null) return;
        if (!pathDrawingMode) return;
        if (rayObj.hit.collider == null) return;
        if (rayObj.hit.collider.gameObject.tag == "floor")
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

    // void HandleMouseInput()
    // {
    //     if (Input.GetKey(KeyCode.Mouse0))
    //     {
    //         MouseHeldFuncs();
    //     }
    //     if (Input.GetKeyUp(KeyCode.Mouse0))
    //     {
    //         MouseUpFuncs();
    //     }
    //     if (Input.GetKeyDown(KeyCode.Mouse0))
    //     {
    //         MouseDownFuncs();
    //     }
    // }

    // void MouseUpFuncs()
    // {
    //     if (pathDrawingMode)
    //     {
    //         StopDrawingPath();
    //     }
    // }

    // void MouseHeldFuncs()
    // {
    //     if (spawnerSource == null) return;
    //     if (!pathDrawingMode) return;
    //     if (rayObj.hit.collider == null) return;
    //     if (rayObj.hit.collider.gameObject.tag == "floor")
    //     {
    //         TryPlaceFollowSphere();
    //     }
    // }

    // void MouseDownFuncs()
    // {
    //     rayObj = rayHandler.RayChecks();
    //     if (rayObj.hit.collider == null) return;

    //     if (IsControlling())
    //     {
    //         controlledUnits[0].AttemptShotAtPosition(new Vector3(rayObj.hit.point.x, 0.5f, rayObj.hit.point.z));
    //     }
    //     else
    //     {
    //         CommandModeMouseDown();
    //     }
    // }

    // void CommandModeMouseDown()
    // {
    //     switch (rayObj.hit.collider.gameObject.tag)
    //     {
    //         case "spawner":
    //             HandleClickOnSpawner();
    //             break;
    //         case "unit":
    //             Debug.Log("Hit unit!");
    //             TryPossessUnit(rayObj.hit.collider.gameObject);
    //             break;
    //         default:
    //             break;
    //     }
    // }

    // void HandleClickOnSpawner()
    // {
    //     if (IsHitObjectSelectedSpawner())
    //     {
    //         HandleClickOnSelectedSpawner();
    //     }
    //     else
    //     {
    //         DeselectSpawners();
    //         SelectSpawner(rayObj.hit.collider.gameObject);
    //     }
    // }

    // void HandleClickOnSelectedSpawner()
    // {
    //     Spawner spawnerClass = spawnerSource.GetComponent<Spawner>();
    //     if (spawnerClass.spawnerPathManager.GetIsDrawable() == true)
    //     {
    //         pathDrawingMode = spawnerClass.spawnerPathManager.GetIsDrawable();
    //         PreparePathBar();
    //     }
    //     else
    //     {
    //         DeselectSpawners();
    //         spawnerSource = null;
    //     }
    // }

    // void DeselectSpawners()
    // {
    //     if (spawnerSource != null)
    //     {
    //         Spawner spawnerClass = spawnerSource.GetComponent<Spawner>();
    //         StopDrawingPath();
    //         spawnerClass.SetUIVisible(false);
    //     }
    // }

    // bool IsHitObjectSelectedSpawner()
    // {
    //     return (spawnerSource == rayObj.hit.collider.gameObject);
    // }

    void SelectSpawner(GameObject spawnerGameObject)
    {
        spawnerSource = spawnerGameObject;
        Spawner spawnerClass = spawnerSource.GetComponent<Spawner>();
        spawnerClass.SetUIVisible(true);
    }

    // Update is called once per frame
    void Update()
    {
        pathHandlerPossessionHandler.GetPossessionMovement();
    }

    public GameObject CreateRedPoint(Vector3 position)
    {
        return Instantiate(prefabRed, position, Quaternion.identity) as GameObject;
    }

    public GameObject CreateBluePoint(Vector3 position)
    {
        return Instantiate(prefabBlue, position, Quaternion.identity) as GameObject;
    }

}
