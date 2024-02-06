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

    public CommandModeInputHandler cmdInputHandler;

    [SerializeField]
    Tilemap tilemap;

    [SerializeField]
    GameObject unitFallingDeathPrefab;

    [SerializeField]
    GameObject grenadeInstancePrefab;

    GameObject grenadeInstance;

    int nullCheck = 0;
    int maxNullCheck = 100;

    // TELEPORT -- SCOUT
    bool teleportLocked = false;

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
                Debug.Log("What");
                FreePossession();
            }
        }
        else
        {
            if (!CommandModeInputHandler.commandLoopEnabled)
            {
                FreePossession();
            }
        }

        if (controlledUnits.Count > 0)
        {
            if (controlledUnits[0] != null)
            {
                if (tilemap == null) return;
                Vector3 unitPos = controlledUnits[0].transform.position;
                Vector3 modifiedSpritePosLeft = new Vector3(unitPos.x + 0.15f, unitPos.y - 0.2f, unitPos.z);
                Vector3 modifiedSpritePosRight = new Vector3(unitPos.x - 0.15f, unitPos.y - 0.2f, unitPos.z);
                Vector3 modifiedSpritePosLeftUp = new Vector3(unitPos.x + 0.15f, unitPos.y + 0.2f, unitPos.z);
                Vector3 modifiedSpritePosRightUp = new Vector3(unitPos.x - 0.15f, unitPos.y + 0.2f, unitPos.z);
                Vector3Int cellLocLeft = tilemap.WorldToCell(modifiedSpritePosLeft);
                cellLocLeft.z = 0;
                Vector3Int cellLocRight = tilemap.WorldToCell(modifiedSpritePosRight);
                cellLocRight.z = 0;
                Vector3Int cellLocLeftUp = tilemap.WorldToCell(modifiedSpritePosLeftUp);
                cellLocLeftUp.z = 0;
                Vector3Int cellLocRightUp = tilemap.WorldToCell(modifiedSpritePosRightUp);
                cellLocRightUp.z = 0;
                if (tilemap.GetTile(cellLocLeft).name == "IndNotFloorTile" && tilemap.GetTile(cellLocRight).name == "IndNotFloorTile" && tilemap.GetTile(cellLocLeftUp).name == "IndNotFloorTile" && tilemap.GetTile(cellLocRightUp).name == "IndNotFloorTile")
                {
                    Debug.Log("DIE");
                    Unit toDieUnit = controlledUnits[0];
                    Instantiate(unitFallingDeathPrefab, toDieUnit.transform.position, toDieUnit.transform.rotation);
                    FreePossession();
                    Destroy(toDieUnit.gameObject);
                }
            }
        }
    }

    public void HeldMouse(Vector2 locationOfMouseCursor)
    {
        if (!UnitSanityCheck())
        {
            return;
        }
        if (IsPossessedUnitInfantry())
        {
            HeldMouseInfantry(locationOfMouseCursor);
        }
        else if (IsPossessedUnitScout())
        {
            HeldMouseScout(locationOfMouseCursor);
        }
    }

    void HeldMouseScout(Vector2 locationOfMouseCursor)
    {
        if (!teleportLocked)
        {
            Vector3 teleportPos = new Vector3(locationOfMouseCursor.x, locationOfMouseCursor.y, Constants.ZED_OFFSET);
            controlledUnits[0].transform.position = teleportPos;
            teleportLocked = true;
        }
    }

    void HeldMouseInfantry(Vector2 locationOfMouseCursor)
    {
        Vector3 grenadePos = new Vector3(locationOfMouseCursor.x, locationOfMouseCursor.y, Constants.ZED_OFFSET);
        if (grenadeInstance == null)
        {
            grenadeInstance = Instantiate(grenadeInstancePrefab, grenadePos, controlledUnits[0].transform.rotation);
            grenadeInstance.transform.localScale = new Vector3(Constants.GRENADE_INITIAL_SCALE, Constants.GRENADE_INITIAL_SCALE, Constants.GRENADE_INITIAL_SCALE);
        }
        else
        {
            grenadeInstance.transform.position = grenadePos;
            if (grenadeInstance.transform.localScale.x < Constants.GRENADE_LOCAL_SCALE)
            {
                grenadeInstance.transform.localScale = grenadeInstance.transform.localScale * 1.01f;
            }
        }
    }

    public void ReleasedMouse()
    {
        if (!UnitSanityCheck())
        {
            Destroy(grenadeInstance);
        }
        else if (grenadeInstance != null)
        {
            if (grenadeInstance.transform.localScale.x >= Constants.GRENADE_LOCAL_SCALE * Constants.GRENADE_MINIMUM_RELEASE_SCALE)
            {
                grenadeInstance.GetComponent<GrenadeInstance>().ReleasedFromPlayer(controlledUnits[0].transform.position);
            }
            else
            {
                Destroy(grenadeInstance);
            }
            grenadeInstance = null;
        }

        if (teleportLocked)
        {
            teleportLocked = false;
        }
    }

    public bool UnitSanityCheck()
    {
        if (controlledUnits.Count > 0)
        {
            if (controlledUnits[0] != null)
            {
                return true;
            }
        }
        return false;
    }

    bool IsPossessedUnitScout()
    {
        if (controlledUnits[0].unitType == Constants.SCOUT_TYPE)
        {
            return true;
        }
        return false;
    }

    bool IsPossessedUnitInfantry()
    {
        if (controlledUnits[0].unitType == Constants.INF_TYPE)
        {
            return true;
        }
        return false;
    }

    public void PossessedMouseDown(Vector2 locationToShootAt)
    {
        if (!UnitSanityCheck()) return;
        controlledUnits[0].PosAttemptShotAtPosition(new Vector3(locationToShootAt.x, locationToShootAt.y, Constants.ZED_OFFSET));
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
        //unitStatUI.SetActive(false);

        SharedPosessionLogic.static_possessedUnit = null;

        CommandModeInputHandler.commandLoopEnabled = true;

        line.SetActive(false);
    }

    // Attempt to possess a unit, going through the various checks and what not.
    public bool TryPossessUnit(GameObject maybePos)
    {
        GameObject potentialUnit = maybePos.transform.parent.gameObject;
        if (potentialUnit.GetComponent<Unit>() == null) return false;

        Unit unit = potentialUnit.GetComponent<Unit>();
        if (unit.unitTeam != teamColor) return false;

        if (controlledUnits.Count > 0)
        {
            controlledUnits[0].beingControlled = false;
        }
        controlledUnits = new List<Unit>();
        PossessUnit(unit);
        return true;
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

        if (pathHandler != null)
        {
            pathHandler.DeselectSpawners();
        }
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
        controlledUnits.Add(unit);
        camScript.followObj = unit.unitObj;

        SharedPosessionLogic.static_possessedUnit = unit;

        line.SetActive(true);
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
                UnitFireCooldownChecker(controlledUnits[0]);
            }
        }
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
