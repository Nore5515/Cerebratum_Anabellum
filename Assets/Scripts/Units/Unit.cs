using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Unit : MonoBehaviour
{

    // Core unit stats
    public int hp { get; set; }
    public int maxHP { get; set; }
    public int dmg { get; set; }
    public int speed { get; set; }
    public float rof { get; set; }
    public int threatLevel { get; set; }

    public bool isUnitInitialized { get; set; }

    // Movement
    public Vector3 direction;

    // UI
    public Slider hpSlider;

    public string unitTeam { get; set; }
    public string unitType { get; set; }
    public float unitRange { get; set; }
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
    Color RED = new Color(255, 0, 0, 0.3f);
    Color BLUE = new Color(0, 0, 255, 0.3f);

    // Point Stuff
    UnitPointHandler unitPointHandler = new UnitPointHandler();
    
    // Consts
    const int MINIMUM_FRAMES_TO_BE_IDLE = 60;
    const float MAX_IDLE_SECONDS = 10.0f;
    public int MIN_DIST_TO_MOVEMENT_DEST = 1;

    int idle_frames = 0;
    float idle_time = 0;
    Vector3 lastPos = new Vector3(0.0f, 0.0f, 0.0f);

    public Vector3 lastAimedTarget;

    // TODO: There is an error within spawner initializing this, and the start function within the
    // classes that extend Unit.
    //
    // This error is that the START function of the extended unit happens the frame AFTER
    // initialization is called.
    //
    // This results in unit-specific variables that are initialized in start being lost.
    //
    // SOLUTIONS:
    // - Constants file, with each unit types stats and such.
    public void Initalize(List<GameObject> newPoints, string newTeam, SpawnedUnitStats newStats)
    {
        unitPossessionHandler = GameObject.Find("PossessionHandler").GetComponent<PosHandler>();
        bulletPrefab = Resources.Load(path) as GameObject;
        unitTeam = newTeam;
        threatState = "WALK";

        if (unitTeam == "RED")
        {
            glow.color = RED;
        }
        else
        {
            glow.color = BLUE;
        }

        rof = newStats.fireDelay;

        unitFiringHandler = gameObject.AddComponent<UnitFiringHandler>();
        unitFiringHandler.Initialize(rof, bulletPrefab, unitTeam, dmg);


        //KillSphere unitKillSphere = GetComponentInChildren(typeof(KillSphere)) as KillSphere;

        detectionSphere.alliedTeam = unitTeam;
        engagementSphere.alliedTeam = unitTeam;

        detectionSphere.GetComponent<SphereCollider>().radius = newStats.unitRange;
        engagementSphere.GetComponent<SphereCollider>().radius = newStats.unitRange + 2;

        unitRange = newStats.unitRange;

        if (newPoints.Count == 0)
        {
            isUnitInitialized = false;
        }
        else
        {
            InitializePoints(newPoints);
        }
    }

    private void InitializePoints(List<GameObject> newPoints)
    {
        unitPointHandler.InitializePoints(newPoints);
    }

    public IEnumerator EnableFiring()
    {
        return unitFiringHandler.EnableFiring();
    }

    // Health and Damage Logic
    public int DealDamage(int damage)
    {
        hp -= damage;
        if (hpSlider != null)
        {
            hpSlider.value = hp;
        }
        return hp;
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
        unitFiringHandler.PosAttemptShotAtPosition(targetPosition);
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
        return unitFiringHandler.canFire;
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
                if (target.GetComponent<Unit>().threatLevel > highestThreat)
                {
                    highestThreat = target.GetComponent<Unit>().threatLevel;
                }
            }
        }
        return highestThreat;
    }

    private string DetermineThreatState(int highestInRangeThreatLevel)
    {
        if (highestInRangeThreatLevel < threatLevel)
        {
            return "WALK";
        }
        else if (highestInRangeThreatLevel == threatLevel)
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
    public void AddPoint(GameObject point)
    {
        unitPointHandler.AddPoint(point);
    }

    public void RemovePoint(GameObject point)
    {
        unitPointHandler.RemovePoint(point, this.transform.position);
    }

    public void RemovePoint(Vector3 point)
    {
        unitPointHandler.RemovePoint(point);
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

        // If an enemy is in range, stay still!
        if (threatState == "STAND" || threatState == "FLEE")
        {
            // Skirmish.
        }
        else
        {
            // Get movement direction.
            direction = getNewMovementVector();

            float distToDest = Vector3.Distance(transform.position, unitPointHandler.DestVector);

            // If you are not close enough to your dest, keep moving towards it.
            if (distToDest >= MIN_DIST_TO_MOVEMENT_DEST)
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
    }

    private Vector3 getNewMovementVector()
    {
        Vector3 newDest = new(unitPointHandler.DestVector.x, transform.position.y, unitPointHandler.DestVector.z);
        Vector3 heading = newDest - transform.position;
        float distance = heading.magnitude;
        return heading / distance;
    }

    private void MoveInDirection(Vector3 directionToMove)
    {
        if (directionToMove.x == float.NaN || directionToMove.y == float.NaN || directionToMove.z == float.NaN)
        {
            Destroy(gameObject);
            return;
        }
        transform.Translate(directionToMove * speed * Time.deltaTime);
    }

    public void PossessedMovement()
    {
        if (controlDirection != new Vector3(0, 0, 0))
        {
            // When controlled, move 50% faster.
            transform.Translate(controlDirection * (speed * 1.5f) * Time.deltaTime);
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
            if (idle_frames > MINIMUM_FRAMES_TO_BE_IDLE)
            {
                idle_time += Time.deltaTime;
                AnimState = "Idle";
                if (idle_time >= MAX_IDLE_SECONDS)
                {
                    if (unitPointHandler.pointVectors.Count <= 0)
                    {
                        Destroy(gameObject);
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