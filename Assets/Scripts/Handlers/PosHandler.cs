using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.CanvasScaler;

public class PosHandler : MonoBehaviour
{

    public Text unitHP;
    public Text unitRoF;
    public Text unitRange;
    public Text unitTitle;
    public Slider cooldownSlider;
    static Unit posUnit;

    public CameraScript camScript;

    public GameObject unitStatUI;

    static float unitMaxDelay;
    static float unitDelay;

    public GameObject line;

    static bool coroutineRunning = false;

    public delegate void UpdateCalls();
    public static UpdateCalls uc;

    private static GameObject unitStats;

    public bool possessionReady = false;

    InputHandler inputHandler;

    public PathHandler pathHandler;

    public string teamColor = "RED";
    public List<Unit> controlledUnits = new List<Unit>();

    bool justFired = false;
    float countdown = 0.0f;

    public void ControlledMouseDown(RayObj rayObj)
    {
        Debug.Log("Attempt shot at point");
        controlledUnits[0].PosAttemptShotAtPosition(new Vector3(rayObj.hit.point.x, 0.5f, rayObj.hit.point.z));
    }

    public void Start()
    {
        inputHandler = new InputHandler(this.gameObject.GetComponent<PosHandler>());
    }

    void Update()
    {
        inputHandler.UpdateFuncs();
    }

    public void SetPossession(bool newPossession)
    {
        possessionReady = newPossession;
    }

    public void FreePossession()
    {
        foreach (var unit in controlledUnits)
        {
            unit.beingControlled = false;
        }
        controlledUnits = new List<Unit>();
        camScript.followObj = null;
        unitStatUI.SetActive(false);
        setPossessed(null);
    }

    public static void setUnitStatUI(GameObject gameObject)
    {
        unitStats = gameObject;
    }

    public bool setPossessed(Unit u)
    {
        posUnit = u;
        if (posUnit != null)
        {
            Debug.Log(posUnit);
            Debug.Log(unitHP);
            unitHP.text = posUnit.hp.ToString();
            unitRoF.text = posUnit.rof.ToString();
            unitRange.text = posUnit.unitRange.ToString();
            unitTitle.text = posUnit.unitType;
            return true;
        }
        else
        {
            unitHP.text = "NAN";
            unitRoF.text = "NAN";
            unitRange.text = "NAN";
            unitTitle.text = "NAN";
            return false;
        }
    }

    // TODO: EVENT DELEGATE THIS
    public void PossessedUnitFired()
    {
        if (!coroutineRunning)
        {
            //unitMaxDelay = posUnit.rof * 0.5f;
            //unitDelay = 0.0f;
            //coroutineRunning = true;
        }
    }

    // Attempt to possess a unit, going through the various checks and what not.
    public void TryPossessUnit(GameObject maybePos)
    {
        if (!Input.GetKey(KeyCode.Space)) return;
        GameObject potentialUnit = maybePos.transform.parent.gameObject;
        if (potentialUnit.GetComponent<Unit>() == null) return;
        Unit unit = potentialUnit.GetComponent<Unit>();
        if (unit.unitTeam != teamColor) return;
        if (controlledUnits.Count > 0)
        {
            controlledUnits[0].beingControlled = false;
        }
        controlledUnits = new List<Unit>();
        PossessUnit(unit);
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
                line.SetActive(true);
                line.GetComponent<LineRenderer>().SetPositions(poss);
            }
            else
            {
                line.SetActive(false);
            }
        }
    }

    // Returns true if you are controlling a unit, and false otherwise.
    public bool IsControlling()
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

    public void GetPossessionMovement()
    {
        if (IsControlling())
        {
            float zMovement = Input.GetAxis("Vertical");
            float xMovement = Input.GetAxis("Horizontal");
            controlledUnits[0].controlDirection = new Vector3(xMovement, 0, zMovement);
        }
    }

    // Possess the passed-in unit.
    void PossessUnit(Unit unit)
    {
        SetPossession(false);
        if (unit == null)
        {
            Debug.LogError("PathHandler.cs --POSSESSED UNIT OBJ DOES NOT HAVE UNIT SCRIPT ATTACHED--");
            return;
        }
        unit.beingControlled = true;
        InitializeCooldownSlider(unit);
        controlledUnits.Add(unit);
        camScript.followObj = unit.unitObj;
        if (!setPossessed(unit))
        {
            Debug.LogError("PathHandler.cs --Possession Handler failed to set possessed unit.--");
            return;
        }
        unitStatUI.SetActive(true);
    }

    void InitializeCooldownSlider(Unit unit)
    {
        cooldownSlider.minValue = 0.0f;
        cooldownSlider.maxValue = unit.rof;
        cooldownSlider.value = cooldownSlider.maxValue;
    }

    // Creates instance
    public PosHandler()
    {
        uc = () => TimedUpdate();
        unitMaxDelay = 0.0f;
        unitDelay = 0.0f;
    }

    // Update is called once per frame
    bool TimedUpdate()
    {
        Debug.Log(unitDelay);
        unitDelay += Time.deltaTime;
        TryUpdateCooldownSlider();
        IterateUnitFireDelay();

        if (unitDelay > unitMaxDelay)
        {
            coroutineRunning = false;
            return false;
        }
        return true;
    }

    private void FixedUpdate()
    {
        TryUpdateCooldownSlider();
        IterateUnitFireDelay();
    }

    void IterateUnitFireDelay()
    {
        if (countdown > 0.0f)
        {
            countdown -= Time.deltaTime;
            if (countdown < 0.0f)
            {
                countdown = 0.0f;
            }
        }
    }

    void TryUpdateCooldownSlider()
    {
        if (controlledUnits.Count > 0)
        {
            if (controlledUnits[0] != null)
            {
                UpdateCooldownSlider(controlledUnits[0]);
            }
        }
    }

    void UpdateCooldownSlider(Unit unit)
    {
        cooldownSlider.value = cooldownSlider.maxValue - countdown;
        UnitFireCooldownChecker(unit);
    }

    void UnitFireCooldownChecker(Unit unit)
    {
        if (unit.canFire == false)
        {
            if (justFired == false)
            {
                justFired = true;
                countdown = unit.rof;
            }
        }
        else
        {
            justFired = false;
        }
    }


}
