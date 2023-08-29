using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Unit : MonoBehaviour
{
    public class TargetHandler
    {
        public List<GameObject> targetsInRange = new List<GameObject>();

        public void ClearNullTargets()
        {
            List<GameObject> toRemoveObjs = new List<GameObject>();
            foreach (GameObject obj in targetsInRange)
            {
                if (obj == null)
                {
                    toRemoveObjs.Add(obj);
                }
            }
            foreach (GameObject markedObj in toRemoveObjs)
            {
                targetsInRange.Remove(markedObj);
            }
        }
    }

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
    // public GameObject spriteRed;
    // public GameObject spriteBlue;

    public string unitTeam { get; set; }
    public string unitType { get; set; }
    public float unitRange { get; set; }
    public bool beingControlled { get; set; }
    public GameObject unitObj { get; set; }
    public GameObject bulletPrefab { get; set; }
    public Vector3 controlDirection { get; set; }

    // STATE
    // TODO: Enum this?
    public string threatState { get; set; }

    // WALK - No threats of equal or higher level.
    // STAND - Threats of equal level, but not higher.
    // FLEE - Threats of higher level.

    string path = "Asset_Projectile";

    //IDictionary<string, GameObject> UnitSprites = new Dictionary<string, GameObject>();
    public string AnimState = "Idle";
    // Idle
    // Shooting
    // Walking

    // Targets Stuff
    public TargetHandler unitTargetHandler = new TargetHandler();

    // Firing Stuff
    public bool canFire { get; set; }
    public bool canFireDelay { get; set; }
    public bool firstFire { get; set; }   // Their first shot should be almost fully charged! 15% normal speed.

    public PosHandler unitPossessionHandler;

    public SpriteRenderer glow;

    // COLORS
    Color RED = new Color(255, 0, 0, 0.3f);
    Color BLUE = new Color(0, 0, 255, 0.3f);

    // Point Stuff
    public List<GameObject> pointObjects = new List<GameObject>();
    public List<Vector3> pointVectors = new List<Vector3>();
    public Vector3 Dest;
    public bool removing = false;

    float MISS_RANGE_RADIUS = 1.0f;
    float CONTROLLED_MISS_RADIUS = 0.0f;
    int MINIMUM_FRAMES_TO_BE_IDLE = 60;
    float MAX_IDLE_SECONDS = 10.0f;
    public int MIN_DIST_TO_MOVEMENT_DEST = 1;

    int idle_frames = 0;
    float idle_time = 0;
    Vector3 lastPos = new Vector3(0.0f, 0.0f, 0.0f);

    // public void Initalize(List<GameObject> newPoints, string newTeam, float _rof, float _unitRange)
    public void Initalize(List<GameObject> newPoints, string newTeam, SpawnedUnitStats newStats)
    {
        unitPossessionHandler = GameObject.Find("PossessionHandler").GetComponent<PosHandler>();
        bulletPrefab = Resources.Load(path) as GameObject;
        MIN_DIST_TO_MOVEMENT_DEST = 1;
        unitTeam = newTeam;
        threatState = "WALK";

        if (unitTeam == "RED")
        {
            glow.color = RED;
        }
        else
        {
            glow.color = BLUE;
            if (gameObject.GetComponentInChildren<SpriteRighter>() != null)
            {
                //gameObject.GetComponentInChildren<SpriteRighter>().Flip();
            }
        }

        rof = newStats.fireDelay;
        firstFire = true;   // First shot ready on initializaiton!
        KillSphere unitKillSphere = GetComponentInChildren(typeof(KillSphere)) as KillSphere;
        unitKillSphere.alliedTeam = unitTeam;
        unitKillSphere.GetComponent<SphereCollider>().radius = newStats.unitRange;
        unitRange = newStats.unitRange;

        if (newPoints.Count == 0)
        {
            isUnitInitialized = false;
        }
        else
        {
            InitializePoints(newPoints);
        }

        // Sprite Stuff
        //UnitSprites.Add("Walking", transform.Find("SpriteRighter/Sprite").gameObject);
        //UnitSprites.Add("Shooting", transform.Find("SpriteRighter/Sprite_shooting").gameObject);
    }

    private void InitializePoints(List<GameObject> newPoints)
    {
        foreach (GameObject newPoint in newPoints)
        {
            pointObjects.Add(newPoint);
            pointVectors.Add(newPoint.transform.position);
        }
        if (pointVectors.Count > 0)
        {
            Dest = pointVectors[0];
        }
    }

    public IEnumerator EnableFiring()
    {
        yield return FireDelay();
        canFire = true;
        canFireDelay = false;
    }

    private WaitForSeconds FireDelay()
    {
        // When controlled, fire 50% faster.
        // Thats a good idea but poorly implemented.
        if (beingControlled && false)
        {
            return new WaitForSeconds(rof * 0.5f);
        }
        else if (firstFire)
        {
            firstFire = false;
            return new WaitForSeconds(rof * 0.15f);
        }
        else
        {
            return new WaitForSeconds(rof);
        }
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

    // FireAt Logic
    // CALLED WHEN POSSESSED
    public virtual void FireAtPosition(Vector3 position, float missRange)
    {
        GameObject bulletInstance = GameObject.Instantiate(bulletPrefab, unitObj.transform.position, Quaternion.identity) as GameObject;
        bulletInstance.transform.LookAt(GetRandomAdjacentPosition(position, missRange));
        bulletInstance.GetComponent<Projectile>().SetProps(new Projectile.Props(unitTeam, dmg));
    }

    // [PARAMS]: Vector3 targetPosition
    // For AI, pass this as target. unitTargetHandler.targetsInRange[0].gameObject.transform.position
    public void AttemptShotAtPosition(Vector3 targetPosition)
    {
        if (canFire)
        {
            ValidAIFireAttempt(targetPosition);
        }
        else
        {
            InvalidFireAttempt();
        }
    }


    private void ValidAIFireAttempt(Vector3 targetPosition)
    {
        canFire = false;
        // Fire with perfect accuracy if controlled.
        float missRange = beingControlled ? CONTROLLED_MISS_RADIUS : MISS_RANGE_RADIUS;
        FireAtPosition(targetPosition, missRange);
    }

    public void PosAttemptShotAtPosition(Vector3 targetPosition)
    {
        if (canFire)
        {
            ValidPosFireAttempt(targetPosition);
        }
        else
        {
            InvalidPosFireAttempt();
        }
    }

    private void ValidPosFireAttempt(Vector3 targetPosition)
    {
        canFire = false;
        if (canFireDelay == false)
        {
            canFireDelay = true;
            StartCoroutine(EnableFiring());
        }
        //unitPossessionHandler.PossessedUnitFired();
        float missRange = beingControlled ? CONTROLLED_MISS_RADIUS : MISS_RANGE_RADIUS;
        FireAtPosition(targetPosition, missRange);
    }

    private void InvalidPosFireAttempt()
    {
        if (canFireDelay == false)
        {
            canFireDelay = true;
            StartCoroutine(EnableFiring());
        }
    }

    private void InvalidFireAttempt()
    {
        //Debug.Log("INVALID FIRE ATTEMPT");
        if (canFireDelay == false)
        {
            canFireDelay = true;
            //Debug.Log("Starting Enable Firing");
            StartCoroutine(EnableFiring());
        }
    }

    public void AddTargetInRange(GameObject target)
    {
        unitTargetHandler.targetsInRange.Add(target);
        ClearNullTargets();
        UpdateThreatState();
        AnimState = "Shooting";
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
            // Sprite = Walking
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
        pointVectors.Add(DuplicateVector(point.transform.position));
        if (pointVectors.Count == 1)
        {
            Dest = pointVectors[0];
        }
    }

    public void RemovePoint(GameObject point)
    {
        pointVectors.Remove(point.transform.position);
        removing = false;
        if (pointVectors.Count == 0)
        {
            Dest = this.transform.position;
        }
        else
        {
            Dest = pointVectors[0];
        }
    }

    public Vector3 DuplicateVector(Vector3 vector)
    {
        Vector3 newVector = new Vector3(vector.x, vector.y, vector.z);
        return newVector;
    }

    public void RemovePoint(Vector3 point)
    {
        pointVectors.Remove(point);
        removing = false;
        if (pointVectors.Count == 0)
        {
            Dest = new Vector3(0.0f, 0.0f, 0.0f);
        }
        else
        {
            Dest = pointVectors[0];
        }
    }

    // Takes in a position and a float (representing randomness)
    // Generates a random position up to randomness away
    // Returns it
    public Vector3 GetRandomAdjacentPosition(Vector3 position, float randomness)
    {
        float randomX = position.x + Random.Range(-randomness, randomness);
        float randomZ = position.z + Random.Range(-randomness, randomness);
        Vector3 random = new Vector3(randomX, position.y, randomZ);
        return random;
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
        if (Dest != new Vector3(0.0f, 0.0f, 0.0f))
        {
            // If an enemy is in range, stay still!
            // if (unitTargetHandler.targetsInRange.Count > 0)
            if (threatState == "STAND" || threatState == "FLEE")
            {
                // Skirmish.
            }
            else
            {
                // Get movement direction.
                var newDest = new Vector3(Dest.x, this.transform.position.y, Dest.z);
                var heading = newDest - this.transform.position;
                var distance = heading.magnitude;
                direction = heading / distance;

                float distToDest = Vector3.Distance(transform.position, Dest);

                // If you are not close enough to your dest, keep moving towards it.
                if (distToDest >= MIN_DIST_TO_MOVEMENT_DEST)
                {
                    // Translate movement.
                    transform.Translate(direction * speed * Time.deltaTime);
                    AnimState = "Walking";
                }
                // Once you get too close to your destination, remove it from your movement path and go towards the next one.
                else
                {
                    if (removing == false)
                    {
                        removing = true;
                        RemovePoint(Dest);
                    }
                }
            }
        }
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
        foreach (GameObject newPoint in newPoints)
        {
            pointObjects.Add(newPoint);
            pointVectors.Add(newPoint.transform.position);
        }
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
                    if (pointVectors.Count <= 0)
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
                    AttemptShotAtPosition(unitTargetHandler.targetsInRange[0].gameObject.transform.position);
                }
            }
        }
        else if (beingControlled)
        {
            PossessedMovement();
        }
    }
}