using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This class will hold two subclasses; UnitStats and UnitLogic.

// Only Unit will be public, and it will primarily just parse requests.
// If someone asks for a Unit type, it will fetch it.
// If someone wants to make the Unit move, Unit will call the correct
// function in Unit Logic.

// This will also hold whatever prefabs are needed.

// Ideally, this will be extended by whatever custom units we design moving forwards.

public class Unit : MonoBehaviour
{
    public UnitStats unitStats = new UnitStats();
    public UnitLogic unitLogic = new UnitLogic();
    public bool isUnitInitialized { get; set; }

    // Movement
    public Vector3 direction;

    // UI
    public Slider hpSlider;

    public bool beingControlled { get; set; }
    public GameObject unitObj { get; set; }
    public GameObject bulletPrefab { get; set; }
    public Vector3 controlDirection { get; set; }

    // STATE
    public string threatState { get; set; }

    // WALK - No threats of equal or higher level.
    // STAND - Threats of equal level, but not higher.
    // FLEE - Threats of higher level.

    string path = "Asset_Projectile";

    public string AnimState = "Idle";
    // Idle
    // Shooting
    // Walking

    // Targets Stuff
    public TargetHandler unitTargetHandler = new TargetHandler();

    // Firing Stuff
    UnitFiringHandler unitFiringHandler;

    public PosHandler unitPossessionHandler;

    public SpriteRenderer glow;

    public KillSphere detectionSphere;
    public EngagementSphere engagementSphere;

    // COLORS


    // Point Stuff
    public UnitPointHandler unitPointHandler = new UnitPointHandler();

    // Consts


    int idle_frames = 0;
    float idle_time = 0;
    Vector3 lastPos = new Vector3(0.0f, 0.0f, 0.0f);

    public Vector3 lastAimedTarget;

    // TEST CODE
    public bool testMode_noPossession = false;

    private void Start()
    {
        SpecializedInitialization();
    }

    public virtual void Die()
    {
        Debug.LogError("Die function not overriden!");
    }

    public virtual void SpecializedInitialization()
    {
    }

    public void Initalize(List<Vector3> newPoints, string newTeam, SpawnedUnitStats newStats)
    {
        Debug.Log("INITIALIZING UNIT!");
        // TEST CODE
        if (!testMode_noPossession)
        {
            unitPossessionHandler = GameObject.Find("PossessionHandler").GetComponent<PosHandler>();
        }

        bulletPrefab = Resources.Load(path) as GameObject;
        unitStats.unitTeam = newTeam;
        threatState = "WALK";

        Debug.Log("Setting new unit team " + newTeam);
        SetGlowColor();

        unitStats.rof = newStats.fireDelay;

        unitFiringHandler = gameObject.AddComponent<UnitFiringHandler>();
        unitFiringHandler.Initialize(unitStats.rof, bulletPrefab, unitStats.unitTeam, unitStats.dmg);

        SetSphereTeams();
        SetSpheresRadius(newStats);

        unitStats.unitRange = newStats.unitRange;

        if (newPoints.Count == 0)
        {
            isUnitInitialized = false;
        }
        else
        {
            InitializePoints(newPoints);
        }
    }

    void SetSpheresRadius(SpawnedUnitStats newStats)
    {
        detectionSphere.GetComponent<SphereCollider>().radius = newStats.unitRange;
        engagementSphere.GetComponent<SphereCollider>().radius = newStats.unitRange + Constants.ENGAGEMENT_SPHERE_RADIUS_MODIFIER;
    }

    void SetGlowColor()
    {
        if (unitStats.unitTeam == Constants.RED_TEAM)
        {
            glow.color = Constants.RED_GLOW_COLOR;
        }
        else
        {
            glow.color = Constants.BLUE_GLOW_COLOR;
        }
    }

    void SetSphereTeams()
    {
        detectionSphere.alliedTeam = unitStats.unitTeam;
        engagementSphere.alliedTeam = unitStats.unitTeam;
    }
    private void InitializePoints(List<Vector3> newPoints)
    {
        unitPointHandler.InitializePoints(newPoints);
    }

    //public IEnumerator EnableFiring()
    //{
    //    return unitFiringHandler.EnableFiring();
    //}

    // Health and Damage Logic
    public int DealDamage(int damage)
    {
        unitStats.hp -= damage;
        if (hpSlider != null)
        {
            hpSlider.value = unitStats.hp;
        }
        return unitStats.hp;
    }

    public void ReceiveDamage(int damage)
    {
        unitStats.hp -= damage;
        if (hpSlider != null)
        {
            hpSlider.value = unitStats.hp;
        }
        if (unitStats.hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    // CALLED WHEN POSSESSED
    public virtual void FireAtPosition(Vector3 position, float missRange)
    {
        unitFiringHandler.FireAtPosition(position, missRange);
    }

    // [PARAMS]: Vector3 targetPosition
    // For AI, pass this as target. unitTargetHandler.targetsInRange[0].gameObject.transform.position
    public void AttemptShotAtPosition(Vector3 targetPosition, bool beingControlled)
    {
        unitFiringHandler.AttemptShotAtPosition(targetPosition, beingControlled);
        lastAimedTarget = targetPosition;
    }

    public void PosAttemptShotAtPosition(Vector3 targetPosition)
    {
        if (unitFiringHandler == null)
        {
            Debug.LogError("HUH");
        }
        else
        {
            unitFiringHandler.PosAttemptShotAtPosition(targetPosition);
        }
    }

    public void AddTargetInRange(GameObject target)
    {
        unitTargetHandler.targetsInRange.Add(target);
        ClearNullTargets();
        UpdateThreatState();
        AnimState = "Shooting";
    }

    public bool GetCanFire()
    {
        if (unitFiringHandler)
        {
            return unitFiringHandler.canFire;
        }
        return false;
    }

    public void UpdateThreatState()
    {
        threatState = DetermineThreatState(GetHighestThreatLevelInRange());
    }

    private int GetHighestThreatLevelInRange()
    {
        int highestThreat = -1;
        foreach (GameObject target in unitTargetHandler.targetsInRange)
        {
            if (target.GetComponent<Unit>() != null)
            {
                if (target.GetComponent<Unit>().unitStats.threatLevel > highestThreat)
                {
                    highestThreat = target.GetComponent<Unit>().unitStats.threatLevel;
                }
            }
        }
        return highestThreat;
    }

    private string DetermineThreatState(int highestInRangeThreatLevel)
    {
        if (highestInRangeThreatLevel < unitStats.threatLevel)
        {
            return "WALK";
        }
        else if (highestInRangeThreatLevel == unitStats.threatLevel)
        {
            return "STAND";
        }
        else
        {
            return "FLEE";
        }
    }

    public void RemoveTargetInRange(GameObject target)
    {
        if (unitTargetHandler.targetsInRange.Contains(target))
        {
            unitTargetHandler.targetsInRange.Remove(target);
            ClearNullTargets();
        }
        if (unitTargetHandler.targetsInRange.Count <= 0)
        {
            AnimState = "Walking";
        }
    }

    public void ClearNullTargets()
    {
        unitTargetHandler.ClearNullTargets();
        UpdateThreatState();
    }


    //
    //   ╔══════════════════════════════════════════════╗
    // ╔══════════════════════════════════════════════════╗
    // ║                                                  ║
    // ║  Point Logic                                     ║
    // ║                                                  ║
    // ╚══════════════════════════════════════════════════╝
    //   ╚══════════════════════════════════════════════╝
    //
    public void AddPoint(Vector3 point)
    {
        unitPointHandler.AddPoint(point);
    }

    public void RemovePoint(Vector3 point)
    {
        unitPointHandler.RemovePoint(point, this.transform.position);
    }

    //
    //   ╔══════════════════════════════════════════════╗
    // ╔══════════════════════════════════════════════════╗
    // ║                                                  ║
    // ║  MOVEMENT                                        ║
    // ║                                                  ║
    // ╚══════════════════════════════════════════════════╝
    //   ╚══════════════════════════════════════════════╝
    // 
    public void AIMovement()
    {
        // If dest exists, cus otherwise you're just stayin' still.
        if (unitPointHandler.DestVector == new Vector3(0.0f, 0.0f, 0.0f)) return;

        switch (threatState)
        {
            case ("STAND"):
                break;
            case ("FLEE"):
                break;
            case ("WALK"):
                WalkingLogic();
                break;
            default:
                break;
        }
    }

    private void WalkingLogic()
    {
        // Get movement direction.
        direction = getNewMovementVector();

        float distToDest = Vector3.Distance(transform.position, unitPointHandler.DestVector);

        // If you are not close enough to your dest, keep moving towards it.
        if (distToDest >= Constants.MIN_DIST_TO_MOVEMENT_DEST)
        {
            // Translate movement.
            MoveInDirection(direction);
            AnimState = "Walking";
        }
        // Once you get too close to your destination, remove it from your movement path and go towards the next one.
        else
        {
            unitPointHandler.AttemptRemoveNextDestPoint();
        }
    }

    private Vector3 getNewMovementVector()
    {
        Vector3 newDest = new Vector3(unitPointHandler.DestVector.x, unitPointHandler.DestVector.y, -0.5f);
        Vector3 heading = newDest - transform.position;
        float distance = heading.magnitude;
        return heading / distance;
    }

    private void MoveInDirection(Vector3 directionToMove)
    {
        Vector3 newDir = directionToMove;
        if (newDir.x == float.NaN || newDir.y == float.NaN || newDir.z == float.NaN)
        {
            Destroy(gameObject);
            return;
        }
        if (this != null)
        {
            transform.Translate(newDir * unitStats.speed * Time.deltaTime);
            Vector3 zedZeroedMovement = transform.position;
            zedZeroedMovement.z = Constants.ZED_OFFSET;
            this.transform.position = zedZeroedMovement;
        }
    }

    public void PossessedMovement()
    {
        if (controlDirection != new Vector3(0, 0, 0))
        {
            // When controlled, move 50% faster.
            transform.Translate(controlDirection * (unitStats.speed * Constants.CONTROLLED_MOVEMENT_MODIFIER) * Time.deltaTime);
            //Debug.Log(speed);
            Vector3 zedZeroedMovement = transform.position;
            zedZeroedMovement.z = Constants.ZED_OFFSET;
            this.transform.position = zedZeroedMovement;
            direction = controlDirection;

        }
    }

    public void UpdatePoints(List<GameObject> newPoints)
    {
        unitPointHandler.UpdatePoints(newPoints);
    }

    public void IdleUpdate()
    {
        if (lastPos == transform.position)
        {
            idle_frames++;
            if (idle_frames > Constants.MINIMUM_FRAMES_TO_BE_IDLE)
            {
                idle_time += Time.deltaTime;
                AnimState = "Idle";
                if (idle_time >= Constants.MAX_IDLE_SECONDS)
                {
                    if (unitPointHandler.pointVectors.Count <= 0)
                    {
                        if (unitStats.unitType != "Scout")
                        {
                            Destroy(gameObject);
                        }
                    }
                }
            }
        }
        else
        {
            idle_frames = 0;
            idle_time = 0;
        }
        lastPos = transform.position;
    }

    public void MovementUpdate()
    {
        if (beingControlled == false)
        {
            AIMovement();

            // If there's a valid target within range!
            if (unitTargetHandler.targetsInRange.Count > 0)
            {
                ClearNullTargets();

                // Are there any targets left after the purge?
                if (unitTargetHandler.targetsInRange.Count > 0)
                {
                    AttemptShotAtPosition(unitTargetHandler.targetsInRange[0].gameObject.transform.position, beingControlled);
                }
            }
        }
        else if (beingControlled)
        {
            PossessedMovement();
        }
    }
}