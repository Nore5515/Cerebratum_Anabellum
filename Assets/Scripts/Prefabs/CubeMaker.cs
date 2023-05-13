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
    GameObject[] spawnerButtons;

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
        if (possessionReady){
            possessionButton.GetComponent<Button>().interactable = false;
            // possessionButton.SetActive(false);
            // unitStatUI.SetActive(true);
        }
        else{
            possessionButton.GetComponent<Button>().interactable = true;
            // possessionButton.SetActive(true);
            // unitStatUI.SetActive(false);
        }
    }

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
        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            SetPossession(true);
        }

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            //
            // ╔══════════════════════════════════════════════════╗
            // ║  Detect if draw has started                      ║
            // ╚══════════════════════════════════════════════════╝
            //
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (controlledUnits.Count >= 1)
                {
                    // Debug.Log("Controlling unit");
                    if (controlledUnits[0] == null)
                    {
                        // Debug.Log("Controlled units [0] == null");
                        controlledUnits = new List<Unit>();
                    }
                    else
                    {
                        controlledUnits[0].ControlledFire(new Vector3(hit.point.x, 0.5f, hit.point.z));
                    }
                }
                else
                {
                    if (hit.collider.gameObject.tag == "spawner")
                    {
                        spawnerSource = hit.collider.gameObject;

                        Debug.Log("Drawing from: " + hit.collider.gameObject.name);

                        Spawner spawnerClass = spawnerSource.GetComponent<Spawner>();
                        // TODO; HAVE THIS CALLED WHEN "drawpath" button pressed.
                        // pathDrawingMode = true;
                        pathDrawingMode = spawnerClass.GetIsDrawable();

                        // UI FLIP
                        spawnerClass.SetUIVisible(!spawnerClass.GetUIVisible());

                        pathBar.value = 0;
                        pathBar.gameObject.SetActive(true);
                        Color green = new Color(88f / 255f, 233f / 255f, 55f / 255f);
                        pathBar.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = green;
                    }
                    else
                    {
                        // Debug.Log("Missed! " + hit.collider);
                        if (spawnerSource != null){
                            Spawner spawnerClass = spawnerSource.GetComponent<Spawner>();
                            spawnerClass.SetUIVisible(false);
                        }
                    }
                }
            }
            else if (Input.GetKeyUp(KeyCode.Mouse0))
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


            DrawLine(hit.point);

            //
            // ╔══════════════════════════════════════════════════╗
            // ║  Various other things.                           ║
            // ╚══════════════════════════════════════════════════╝
            //
            if (Input.GetKey(KeyCode.Mouse0))
            {
                //
                // ╔══════════════════════════════════════════════════╗
                // ║  Draw Paths.                                     ║
                // ╚══════════════════════════════════════════════════╝
                //
                if (spawnerSource != null)
                {
                    if (pathDrawingMode)
                    {
                        if (hit.collider.gameObject.tag == "floor")
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
                    }
                }

                //
                // ╔══════════════════════════════════════════════════╗
                // ║  Possess unit on click.                          ║
                // ╚══════════════════════════════════════════════════╝
                //
                if (possessionReady){
                    SetPossession(false);
                    if (controlledUnits.Count == 0){
                        if (hit.collider.gameObject.tag == "unit"){
                            Unit unit = hit.collider.gameObject.GetComponent<Unit>();
                            if (unit != null)
                            {
                                // Confirm unit is same team.
                                if (unit.team == teamColor)
                                {
                                    if (controlledUnits.Count >= 1)
                                    {
                                        controlledUnits[0].beingControlled = false;
                                        controlledUnits = new List<Unit>();
                                    }
                                    unit.beingControlled = true;
                                    controlledUnits.Add(unit);
                                    camScript.followObj = unit.unitObj;
                                    PossessionHandler.setPossessed(unit);
                                    unitStatUI.SetActive(true);
                                    possessionButton.SetActive(false);
                                }
                            }
                        }
                    }
                }
            }
            
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
            PossessionHandler.setPossessed(null);
        }

        if (controlledUnits.Count >= 1)
        {
            if (controlledUnits[0] != null)
            {
                float zMovement = Input.GetAxis("Vertical");
                float xMovement = Input.GetAxis("Horizontal");
                controlledUnits[0].controlDirection = new Vector3(xMovement, 0, zMovement);

                
            }
        }
    }

    public void DrawLine(Vector3 target)
    {
        if (target != null){
            if (controlledUnits.Count > 0){
                if (controlledUnits[0] != null){
                    Vector3 pos1 = controlledUnits[0].transform.position;
                    Vector3 pos2 = target;
                    Vector3[] poss = {pos1, pos2};
                    line.GetComponent<LineRenderer>().SetPositions(poss);
                }
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
