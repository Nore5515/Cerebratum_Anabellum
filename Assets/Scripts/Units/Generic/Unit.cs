using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

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
    public bool initialMove = true;

    // UI
    public Slider hpSlider;

    public bool beingControlled { get; set; }
    public GameObject unitObj { get; set; }
    public virtual GameObject bulletPrefab { get; set; }
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
        var agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        SpecializedInitialization();
    }

    public virtual void Die(string causeOfDeath)
    {
        Destroy(gameObject);
    }

    public virtual void SpecializedInitialization()
    {
    }

    public void UpdateUnitFiringHandler(GameObject newBullet)
    {
        unitFiringHandler.bulletPrefab = newBullet;
    }

    public void Initalize(List<Vector3> newPoints, string newTeam, SpawnedUnitStats newStats)
    {
        //Debug.Log("INITIALIZING UNIT!");
        // TEST CODE
        if (!testMode_noPossession)
        {
            unitPossessionHandler = GameObject.Find("PossessionHandler").GetComponent<PosHandler>();
        }

        bulletPrefab = Resources.Load(path) as GameObject;
        unitStats.unitTeam = newTeam;
        threatState = "WALK";

        //Debug.Log("Setting new unit team " + newTeam);
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
        if (detectionSphere.GetComponent<SphereCollider>() != null)
        {
            detectionSphere.GetComponent<SphereCollider>().radius = newStats.unitRange;
            engagementSphere.GetComponent<SphereCollider>().radius = newStats.unitRange + Constants.ENGAGEMENT_SPHERE_RADIUS_MODIFIER;
        }
        if (detectionSphere.GetComponent<CircleCollider2D>() != null)
        {
            detectionSphere.GetComponent<CircleCollider2D>().radius = newStats.unitRange;
            engagementSphere.GetComponent<CircleCollider2D>().radius = newStats.unitRange + Constants.ENGAGEMENT_SPHERE_RADIUS_MODIFIER;
        }
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

    public void ReceiveDamage(int damage, string sourceOfDamage)
    {
        unitStats.hp -= damage;
        if (hpSlider != null)
        {
            hpSlider.value = unitStats.hp;
        }
        if (unitStats.hp <= 0)
        {
            Die(sourceOfDamage);
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
        Debug.Log("Adding target");
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

        if (initialMove)
        {
            AIWalkingLogic();
        }
        else
        {
            switch (threatState)
            {
                case ("STAND"):
                    break;
                case ("FLEE"):
                    break;
                case ("WALK"):
                    AIWalkingLogic();
                    break;
                default:
                    break;
            }
        }
    }

    bool setDest = false;
    void AIWalkingLogic()
    {
        if (GetComponent<NavMeshAgent>().isStopped)
        {
            GetComponent<NavMeshAgent>().isStopped = false;
        }
        if (!GetComponent<NavMeshAgent>().pathPending && setDest == false)
        {
            if (unitPointHandler.pointVectors.Count > 0)
            {
                float distToDest = Vector3.Distance(transform.position, unitPointHandler.DestVector);
                if (distToDest <= Constants.MIN_DIST_TO_MOVEMENT_DEST)
                {
                    Debug.Log("Moving on to next dest");
                    unitPointHandler.AttemptRemoveNextDestPoint();
                }
                else
                {
                    GetComponent<NavMeshAgent>().SetDestination(unitPointHandler.pointVectors[0]);
                }
                return;
            }

            Debug.Log("Set Dest!");
            List<GameObject> hqObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("hq"));
            List<GameObject> enemyHQs = new List<GameObject>();

            foreach (GameObject hq in hqObjects)
            {
                if (hq.GetComponent<HQObject>().team != unitStats.unitTeam)
                {
                    enemyHQs.Add(hq);
                }
            }
            if (enemyHQs.Count <= 0)
            {
                Debug.LogError("No HQs found by crate!");
            }

            GetComponent<NavMeshAgent>().SetDestination(enemyHQs[0].transform.position);
            setDest = true;
        }
    }

    private Vector3 getNewMovementVector()
    {
        Vector3 newDest = new Vector3(unitPointHandler.DestVector.x, unitPointHandler.DestVector.y, -0.5f);
        Vector3 heading = newDest - transform.position;
        float distance = heading.magnitude;
        return heading / distance;
    }

    [SerializeField]
    float mod = 1.0f;
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
            if (this.GetComponent<Rigidbody2D>() != null)
            {
                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                Vector2 velo = newDir * unitStats.speed * Time.deltaTime * mod;
                rb.MovePosition(rb.position + velo);
            }
            else
            {
                Debug.LogError("NO RIGID BODY ON UNIT");
                //transform.Translate(newDir * unitStats.speed * Time.deltaTime);
                //Vector3 zedZeroedMovement = transform.position;
                //zedZeroedMovement.z = Constants.ZED_OFFSET;
                //this.transform.position = zedZeroedMovement;
            }
        }
    }

    public void PossessedMovement()
    {
        if (!GetComponent<NavMeshAgent>().isStopped)
        {
            GetComponent<NavMeshAgent>().isStopped = true;
        }
        if (controlDirection != new Vector3(0, 0, 0))
        {
            Debug.Log(controlDirection);
            //MoveInDirection(controlDirection);
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
                        if (unitStats.unitType != Constants.SCOUT_TYPE && unitStats.unitType != Constants.TURRET_TYPE)
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
        Debug.Log("Being Controlled: " + beingControlled);
        if (!beingControlled)
        {
            AIMovement();

            // If there's a valid target within range!
            if (unitTargetHandler.targetsInRange.Count > 0)
            {
                ClearNullTargets();

                // Are there any targets left after the purge?
                if (unitTargetHandler.targetsInRange.Count > 0)
                {
                    Debug.Log("Firing!");
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