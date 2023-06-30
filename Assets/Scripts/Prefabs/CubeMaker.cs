using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CubeMaker : MonoBehaviour
{
    RayHandler rayHandler;
    RayObj rayObj = new RayObj();

    public GameObject prefabRed;
    public GameObject prefabBlue;

    public string teamColor = "RED";
    public Text teamColorText;

    // public List<GameObject> toRemoveUnits = new List<GameObject>();
    public List<Unit> controlledUnits = new List<Unit>();

    public Vector3 unitDirection = new Vector3();
    public CameraScript camScript;

    public float distancePerSphere = 0.0f;
    public float maxDistancePerSphere = 5.0f;
    public Vector3 oldPos = new Vector3();

    // NEW STUFF
    Color red = new Color(233f / 255f, 80f / 255f, 55f / 255f);

    public bool pathDrawingMode = false;
    public bool possessionReady = false;

    public Slider pathBar;
    private Image pathBarFill;
    public GameObject possessionButton;

    GameObject spawnerSource;

    public GameObject line;
    public GameObject unitStatUI;

    public PossessionHandler cubeMakerPossessionHandler;

    public void Start()
    {
        rayHandler = new RayHandler();
        pathBarFill = pathBar.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>();

        possessionButton = GameObject.Find("PossessButton");
        PossessionHandler.setUnitStatUI(GameObject.Find("Canvas/UnitStats"));
        GameObject.Find("Canvas/UnitStats").SetActive(false);
    }

    public void SetPathDrawingMode(bool newMode)
    {
        pathDrawingMode = newMode;
    }

    public void SetPossession(bool newPossession)
    {
        possessionReady = newPossession;
        possessionButton.GetComponent<Button>().interactable = !possessionReady;
    }

    void KeyChecks()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            SetPossession(true);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            FreePossession();
        }
    }

    void FreePossession()
    {
        foreach (var unit in controlledUnits)
        {
            unit.beingControlled = false;
        }
        controlledUnits = new List<Unit>();
        camScript.followObj = null;
        unitStatUI.SetActive(false);
        possessionButton.SetActive(true);
        cubeMakerPossessionHandler.setPossessed(null);
    }

    // Attempt to place a sphere down on where the raycast hits the world.
    void TryPlaceFollowSphere()
    {
        if (distancePerSphere >= maxDistancePerSphere)
        {
            PlaceFollowSphere();
        }

        // If distance is less than maxdistancepersphere, add change in distance.
        else
        {
            AddSphereDistance();
        }
    }

    void PlaceFollowSphere()
    {
        Spawner spawnerClass = spawnerSource.GetComponent<Spawner>();

        distancePerSphere = 0.0f;

        spawnerClass.DrawPathSphereAtPoint(rayObj.hit.point, ref pathBar);
    }

    void AddSphereDistance()
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

    // Returns true if you are controlling a unit, and false otherwise.
    bool IsControlling()
    {
        if (controlledUnits.Count >= 1)
        {
            if (controlledUnits[0] == null)
            {
                controlledUnits = new List<Unit>();
                return false;
            }
            else
            {
                return true;
            }
        }
        return false;
    }

    // Possess the passed-in unit.
    void PossessUnit(Unit unit)
    {
        SetPossession(false);
        if (unit == null)
        {
            Debug.LogError("CubeMaker.cs --POSSESSED UNIT OBJ DOES NOT HAVE UNIT SCRIPT ATTACHED--");
            return;
        }
        unit.beingControlled = true;
        controlledUnits.Add(unit);
        camScript.followObj = unit.unitObj;
        if (!cubeMakerPossessionHandler.setPossessed(unit))
        {
            Debug.LogError("CubeMaker.cs --Possession Handler failed to set possessed unit.--");
            return;
        }
        unitStatUI.SetActive(true);
        possessionButton.SetActive(false);
    }

    void PreparePathBar()
    {
        pathBar.value = 0;
        pathBar.gameObject.SetActive(true);
        Color green = new Color(88f / 255f, 233f / 255f, 55f / 255f);
        pathBar.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = green;
    }

    // Attempt to possess a unit, going through the various checks and what not.
    void TryPossessUnit(GameObject maybePos)
    {
        if (!possessionReady) return;
        if (maybePos.GetComponent<Unit>() == null) return;
        Unit unit = maybePos.GetComponent<Unit>();
        if (unit.unitTeam != teamColor) return;
        if (controlledUnits.Count == 0) return;
        controlledUnits[0].beingControlled = false;
        controlledUnits = new List<Unit>();
        PossessUnit(unit);
    }

    void StopDrawingPath()
    {
        pathDrawingMode = false;
        pathBar.gameObject.SetActive(false);
        if (spawnerSource != null)
        {
            spawnerSource.GetComponent<Spawner>().SetIsDrawable(false);
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

    void HandleMouseInput()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            MouseHeldFuncs();
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            MouseUpFuncs();
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            MouseDownFuncs();
        }
    }

    void MouseUpFuncs()
    {
        if (pathDrawingMode)
        {
            StopDrawingPath();
        }
    }

    void MouseHeldFuncs()
    {
        if (spawnerSource == null) return;
        if (!pathDrawingMode) return;
        if (rayObj.hit.collider == null) return;
        if (rayObj.hit.collider.gameObject.tag == "floor")
        {
            TryPlaceFollowSphere();
        }
    }

    void MouseDownFuncs()
    {
        rayObj = rayHandler.RayChecks();
        if (rayObj.hit.collider == null) return;

        if (IsControlling())
        {
            controlledUnits[0].AttemptShotAtPosition(new Vector3(rayObj.hit.point.x, 0.5f, rayObj.hit.point.z));
        }
        else
        {
            CommandModeMouseDown();
        }
    }

    void CommandModeMouseDown()
    {
        switch (rayObj.hit.collider.gameObject.tag)
        {
            case "spawner":
                HandleClickOnSpawner();
                break;
            case "unit":
                Debug.Log("Hit unit!");
                TryPossessUnit(rayObj.hit.collider.gameObject);
                break;
            default:
                break;
        }
    }

    void HandleClickOnSpawner()
    {
        if (IsHitObjectSelectedSpawner())
        {
            HandleClickOnSelectedSpawner();
        }
        else
        {
            DeselectSpawners();
            SelectSpawner(rayObj.hit.collider.gameObject);
        }
    }

    void HandleClickOnSelectedSpawner()
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

    void DeselectSpawners()
    {
        if (spawnerSource != null)
        {
            Spawner spawnerClass = spawnerSource.GetComponent<Spawner>();
            StopDrawingPath();
            spawnerClass.SetUIVisible(false);
        }
    }

    bool IsHitObjectSelectedSpawner()
    {
        return (spawnerSource == rayObj.hit.collider.gameObject);
    }

    void SelectSpawner(GameObject spawnerGameObject)
    {
        spawnerSource = spawnerGameObject;
        Spawner spawnerClass = spawnerSource.GetComponent<Spawner>();
        spawnerClass.SetUIVisible(true);
    }

    void UpdateCalledFuncs()
    {
        rayObj = rayHandler.RayChecks();
        HandleMouseInput();
        DrawLine(rayObj.hit.point);
    }

    void GetPossessionMovement()
    {
        if (IsControlling())
        {
            float zMovement = Input.GetAxis("Vertical");
            float xMovement = Input.GetAxis("Horizontal");
            controlledUnits[0].controlDirection = new Vector3(xMovement, 0, zMovement);
        }
    }

    // Update is called once per frame
    void Update()
    {
        KeyChecks();
        UpdateCalledFuncs();
        GetPossessionMovement();
    }

    public void DrawLine(Vector3 target)
    {
        if (target != null)
        {
            if (IsControlling())
            {
                Vector3 pos1 = controlledUnits[0].transform.position;
                Vector3 pos2 = target;
                Vector3[] poss = { pos1, pos2 };
                line.GetComponent<LineRenderer>().SetPositions(poss);
            }
        }
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
