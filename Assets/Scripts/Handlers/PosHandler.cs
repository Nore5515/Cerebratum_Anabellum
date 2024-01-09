using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
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

    public delegate void UpdateCalls();
    public static UpdateCalls uc;

    public bool possessionReady = false;

    PossessionInputHandler posInputHandler;

    public PathHandler pathHandler;

    public string teamColor = "RED";
    public List<Unit> controlledUnits = new();

    bool justFired = false;
    float countdown = 0.0f;

    public bool possessionKeyHeld = false;

    public CommandModeInputHandler cmdInputHandler;

    [SerializeField]
    Tilemap tilemap;

    public void Start()
    {
        posInputHandler = new PossessionInputHandler(this, tilemap);
    }

    void Update()
    {
        posInputHandler.UpdateFuncs();

        if (controlledUnits.Count > 0)
        {
            if (controlledUnits[0] == null)
            {
                FreePossession();
            }
        }
    }

    public void ControlledMouseDown(RayObj rayObj)
    {
        controlledUnits[0].PosAttemptShotAtPosition(new Vector3(rayObj.hit.point.x, rayObj.hit.point.y, Constants.ZED_OFFSET));
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
        SetNoPosUnitUI();
    }

    private void SetPosUnitUI(Unit posUnit)
    {
        unitHP.text = posUnit.hp.ToString();
        unitRoF.text = posUnit.rof.ToString();
        unitRange.text = posUnit.unitRange.ToString();
        unitTitle.text = posUnit.unitType;
    }

    private void SetNoPosUnitUI()
    {
        unitHP.text = "NAN";
        unitRoF.text = "NAN";
        unitRange.text = "NAN";
        unitTitle.text = "NAN";
    }

    // Attempt to possess a unit, going through the various checks and what not.
    public void TryPossessUnit(GameObject maybePos)
    {
        // NOT A GOOD FIX
        if (!possessionKeyHeld) return;

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
        if (target != null && line != null)
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
        if (controlledUnits.Count == 0) return false;
        if (controlledUnits[0] == null)
        {
            controlledUnits = new List<Unit>();
            return false;
        }
        pathHandler.DeselectSpawners();
        return true;
    }

    public void GetPossessionMovement()
    {
        if (!IsControlling()) return;

        float yMovement = Input.GetAxis("Vertical");
        float xMovement = Input.GetAxis("Horizontal");
        controlledUnits[0].controlDirection = new Vector3(xMovement, yMovement, 0.0f);
    }

    // Possess the passed-in unit.
    void PossessUnit(Unit unit)
    {
        SetPossession(false);

        if (unit == null) return;

        unit.beingControlled = true;
        //InitializeCooldownSlider(unit);
        controlledUnits.Add(unit);
        camScript.followObj = unit.unitObj;

        if (unit == null)
        {
            //SetNoPosUnitUI();
        }
        else
        {
            //SetPosUnitUI(unit);
            //unitStatUI.SetActive(true);
        }
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
                //UpdateCooldownSlider(controlledUnits[0]);
                //TEMP
                UnitFireCooldownChecker(controlledUnits[0]);
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
        if (unit.GetCanFire() == false)
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
