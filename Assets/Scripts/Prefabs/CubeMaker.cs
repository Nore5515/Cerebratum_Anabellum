using System.Collections;
using System.Collections.Generic;
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
    bool thingy;


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

    public Slider pathBar;
    private Image pathBarFill;

    GameObject spawnerSource;

    public void Start()
    {
        spawnerButtons = GameObject.FindGameObjectsWithTag("spawner_buttons");
        pathBar.maxValue = maxCount;
        pathBarFill = pathBar.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>();
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
            SceneManager.LoadScene("DemoMenu");
        }
        if (thingy == false)
        {
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
                    if (hit.collider.gameObject.tag == "spawner")
                    {

                        // turn on all spawner option buttons
                        // Hide all Spawner buttons
                        foreach (var button in spawnerButtons)
                        {
                            button.SetActive(true);
                        }

                        spawnerSource = hit.collider.gameObject;
                        // Debug.Log("Drawing from: ");
                        // Debug.Log(hit.collider.gameObject.name);
                        Spawner spawnerClass = spawnerSource.GetComponent<Spawner>();
                        while (spawnerClass.spheres.Count > 0)
                        {
                            spawnerClass.RemovePoint(spawnerClass.spheres[0]);
                        }
                        pathBar.value = 0;
                        pathBar.gameObject.SetActive(true);
                        Color green = new Color(88f / 255f, 233f / 255f, 55f / 255f);
                        pathBar.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = green;
                    }
                    else
                    {
                        // Hide all Spawner buttons
                        foreach (var button in spawnerButtons)
                        {
                            button.SetActive(false);
                        }
                    }
                }
                else if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    // You ain't drawing if your not pressing down
                    spawnerSource = null;
                    // Hide pathbar when not in use
                    pathBar.gameObject.SetActive(false);
                }

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
                        if (hit.collider.gameObject.tag == "floor")
                        {
                            if (distancePerSphere >= maxDistancePerSphere)
                            {
                                Spawner spawnerClass = spawnerSource.GetComponent<Spawner>();
                                
                                distancePerSphere = 0.0f;
                                GameObject obj;

                                if (teamColor == "RED")
                                {
                                    obj = Instantiate(prefabRed, hit.point, Quaternion.identity) as GameObject;
                                    spawnerClass.spheres.Add(obj);
                                    pathBar.value = spawnerClass.spheres.Count;
                                    if (spawnerClass.spheres.Count == maxCount)
                                    {
                                        pathBarFill.color = red;
                                    }
                                }
                                else
                                {
                                    obj = Instantiate(prefabBlue, new Vector3(hit.point.x, hit.point.y, hit.point.z), Quaternion.identity) as GameObject;
                                    spawnerClass.spheres.Add(obj);
                                }
                                foreach (GameObject unit in spawnerClass.instances)
                                {
                                    if (unit != null)
                                    {
                                        if (unit.GetComponent<Unit>().team == teamColor)
                                        {
                                            unit.GetComponent<Unit>().AddPoint(obj);
                                        }
                                    }
                                    else
                                    {
                                        toRemoveUnits.Add(unit);
                                    }
                                }
                                foreach (GameObject markedUnit in toRemoveUnits)
                                {
                                    spawnerClass.instances.Remove(markedUnit);
                                }
                                toRemoveUnits = new List<GameObject>();

                                if (spawnerClass.spheres.Count >= maxCount){
                                    spawnerClass.RemovePoint(spawnerClass.spheres[0]);
                                }
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

                    //
                    // ╔══════════════════════════════════════════════════╗
                    // ║  Possess unit on click.                          ║
                    // ╚══════════════════════════════════════════════════╝
                    //
                    if (controlledUnits.Count == 0){
                        if (hit.collider.gameObject.tag == "unit")
                        {
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
                                }
                            }
                        }
                    }
                }
                else if (controlledUnits.Count >= 1)
                {
                    if (controlledUnits[0] == null)
                    {
                        controlledUnits = new List<Unit>();
                    }
                    else
                    {
                        if (Input.GetKey(KeyCode.Mouse0))
                        {
                            // Shoot here.
                            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                            if (Physics.Raycast(ray, out hit))
                            {
                                controlledUnits[0].ControlledFire(new Vector3(hit.point.x, 0.5f, hit.point.z));
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

    public GameObject CreateRedPoint(Vector3 position)
    {
        return Instantiate(prefabRed, position, Quaternion.identity) as GameObject;
    }

    public GameObject CreateBluePoint(Vector3 position)
    {
        return Instantiate(prefabBlue, position, Quaternion.identity) as GameObject;
    }

}
