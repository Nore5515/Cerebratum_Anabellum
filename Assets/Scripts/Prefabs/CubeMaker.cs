using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CubeMaker : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;
    public GameObject prefabRed;
    public GameObject prefabBlue;
    public int maxCount;

    public string teamColor = "RED";
    public Text teamColorText;

    public List<GameObject> toRemoveUnits = new List<GameObject>();
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

    public PossessionHandler ph;

    public void Start()
    {
        pathBar.maxValue = maxCount;
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
        if (possessionReady)
        {
            possessionButton.GetComponent<Button>().interactable = false;
        }
        else
        {
            possessionButton.GetComponent<Button>().interactable = true;
        }
    }

    void KeyChecks()
    {
        // if (Input.GetKey(KeyCode.Alpha1))
        // {
        //     teamColor = "RED";
        //     teamColorText.text = teamColor;
        // }
        // if (Input.GetKey(KeyCode.Alpha2))
        // {
        //     teamColor = "BLUE";
        //     teamColorText.text = teamColor;
        // }
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
            foreach (var unit in controlledUnits)
            {
                unit.beingControlled = false;
            }
            controlledUnits = new List<Unit>();
            camScript.followObj = null;
            unitStatUI.SetActive(false);
            possessionButton.SetActive(true);
            ph.setPossessed(null);
        }
    }

    // Attempt to place a sphere down on where the raycast hits the world.
    void TryPlaceFollowSphere()
    {
        if (distancePerSphere >= maxDistancePerSphere)
        {
            Spawner spawnerClass = spawnerSource.GetComponent<Spawner>();

            distancePerSphere = 0.0f;

            spawnerClass.DrawPathAtPoint(hit.point, maxCount, ref pathBar);
        }

        // If distance is less than maxdistancepersphere, add change in distance.
        else
        {
            if (oldPos == new Vector3(0.0f, 0.0f, 0.0f))
            {
                oldPos = hit.collider.gameObject.transform.position;
            }
            else
            {
                distancePerSphere += Vector3.Distance(oldPos, hit.point);
                oldPos = hit.point;
            }
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
                Debug.Log("TRUE");
                return true;
            }
        }
        return false;
    }

    // Possess the passed-in unit.
    void PossessUnit(Unit unit)
    {
        SetPossession(false);
        if (unit != null)
        {
            unit.beingControlled = true;
            controlledUnits.Add(unit);
            camScript.followObj = unit.unitObj;
            if (ph.setPossessed(unit))
            {
                unitStatUI.SetActive(true);
                possessionButton.SetActive(false);
            }
            else
            {
                Debug.LogError("CubeMaker.cs --Possession Handler failed to set possessed unit.--");
            }
        }
        else
        {
            Debug.LogError("CubeMaker.cs --POSSESSED UNIT OBJ DOES NOT HAVE UNIT SCRIPT ATTACHED--");
        }
    }

    // Things that happen when you click mouse down.
    void MouseDownFuncs()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (hit.collider != null)
            {
                // While controlling a unit, clicking = firing.
                if (IsControlling())
                {
                    controlledUnits[0].ControlledFire(new Vector3(hit.point.x, 0.5f, hit.point.z));
                }
                else
                {
                    switch (hit.collider.gameObject.tag)
                    {
                        case "spawner":
                            // YOU JUST CLICKED ON A SPAWNER!!
                            // If there is NO SELECTED SPAWNER, select it.
                            // If there is already a selected spawner...
                            //      If the spawner YOU CLICKED is the selected spawner...
                            //          If the spawnerClass is Drawable, begin drawing!
                            //          Otherwise, deselect.
                            //      If the spawner YOU CLICKED is NOT the selected spawner, make it the selected spawner!

                            // Update Spawner to clicked Spawner
                            if (spawnerSource == null)
                            {
                                spawnerSource = hit.collider.gameObject;
                                Spawner spawnerClass = spawnerSource.GetComponent<Spawner>();
                                spawnerClass.SetUIVisible(true);
                            }
                            else
                            {
                                if (spawnerSource == hit.collider.gameObject)
                                {
                                    Spawner spawnerClass = spawnerSource.GetComponent<Spawner>();
                                    if (spawnerClass.GetIsDrawable() == true)
                                    {
                                        Debug.Log("Drawing!");
                                        pathDrawingMode = spawnerClass.GetIsDrawable();
                                        PreparePathBar();
                                    }
                                    else
                                    {
                                        Debug.Log("Deselecting.");
                                        DeselectSpawners();
                                        spawnerSource = null;
                                    }
                                }
                            }
                            break;
                        case "unit":
                            TryPossessUnit(hit.collider.gameObject);
                            break;
                        case "floor":
                            // DeselectSpawners();
                            break;
                        default:
                            break;
                    }
                }
            }
            // Clicked on nothing should de-select all.
            else
            {
                // DeselectSpawners();
            }
        }
    }

    void DeselectSpawners()
    {
        if (spawnerSource != null)
        {
            Spawner spawnerClass = spawnerSource.GetComponent<Spawner>();
            pathDrawingMode = false;
            spawnerClass.SetUIVisible(false);
        }
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
        if (possessionReady)
        {
            Unit unit = maybePos.GetComponent<Unit>();
            if (unit != null)
            {
                if (unit.team == teamColor)
                {
                    if (controlledUnits.Count >= 1)
                    {
                        controlledUnits[0].beingControlled = false;
                        controlledUnits = new List<Unit>();
                    }
                    PossessUnit(unit);
                }
            }
            else
            {
                Debug.LogError("Cube Maker: TryPossessUnit --- GameObject passed did not have a unit script.");
            }
        }
    }

    // Things that happen when you release the mouse button.
    void MouseUpFuncs()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (pathDrawingMode == true)
            {
                // You ain't drawing if your not pressing down
                // spawnerSource = null;
                pathDrawingMode = false;
                if (spawnerSource != null)
                {
                    spawnerSource.GetComponent<Spawner>().SetIsDrawable(false);
                }
                // Hide pathbar when not in use
                pathBar.gameObject.SetActive(false);
            }
        }
    }

    // Things that happen while you hold down the mouse button.
    void MouseHeldFuncs()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            //
            // ╔══════════════════════════════════════════════════╗
            // ║  Draw Paths.                                     ║
            // ╚══════════════════════════════════════════════════╝
            //
            if (spawnerSource != null)
            {
                Debug.Log("Spawner Source is not null.");
                if (pathDrawingMode)
                {
                    Debug.Log("Path drawing mode is a go.");
                    if (hit.collider != null)
                    {
                        Debug.Log("raycast has hit something.");
                        if (hit.collider.gameObject.tag == "floor")
                        {
                            Debug.Log("raycast has hit valid target");
                            TryPlaceFollowSphere();
                        }
                    }
                }
            }
        }
    }

    void RayChecks()
    {
        int layerMask = 1 << 6;
        layerMask |= (1 << 2);
        layerMask = ~layerMask;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask);
        MouseDownFuncs();
        MouseUpFuncs();
        MouseHeldFuncs();
        DrawLine(hit.point);
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
        RayChecks();
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
