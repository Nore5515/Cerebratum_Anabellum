using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Unit : MonoBehaviour
{

    public class TargetHandler
    {
        public List<GameObject> targetsInRange = new List<GameObject>();
    }

    // Core unit stats
    public int hp { get; set; }
    public int maxHP { get; set; }
    public int dmg { get; set; }
    public int speed { get; set; }
    public float rof { get; set; }
    public int threatLevel { get; set; }

    // Range (REFACTOR TODO: TODO;)
    public int MIN_DIST_TO_MOVEMENT_DEST = 1;

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

    IDictionary<string, GameObject> UnitSprites = new Dictionary<string, GameObject>();

    // Targets Stuff
    // public List<GameObject> th.targetsInRange = new List<GameObject>();
    public TargetHandler th = new TargetHandler();

    // Firing Stuff
    public bool canFire { get; set; }
    public bool canFireDelay { get; set; }
    public bool firstFire { get; set; }   // Their first shot should be almost fully charged! 15% normal speed.

    public PossessionHandler unitPossessionHandler;

    public SpriteRenderer glow;

    // COLORS
    Color RED = new Color(255, 0, 0, 0.3f);
    Color BLUE = new Color(0, 0, 255, 0.3f);

    // Point Stuff
    public List<GameObject> points = new List<GameObject>();
    public GameObject Dest;
    public bool removing = false;

    float MISS_RANGE_RADIUS = 1.0f;
    float CONTROLLED_MISS_RADIUS = 0.0f;


    public void Initalize(List<GameObject> newPoints, string newTeam, float _rof, float _unitRange)
    {
        unitPossessionHandler = GameObject.Find("PossessionHandler").GetComponent<PossessionHandler>();
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
                gameObject.GetComponentInChildren<SpriteRighter>().Flip();
            }
        }

        rof = _rof;
        firstFire = true;   // First shot ready on initializaiton!
        KillSphere unitKillSphere = GetComponentInChildren(typeof(KillSphere)) as KillSphere;
        unitKillSphere.alliedTeam = unitTeam;
        unitKillSphere.GetComponent<SphereCollider>().radius = _unitRange;
        unitRange = _unitRange;

        foreach (GameObject newPoint in newPoints)
        {
            points.Add(newPoint);
        }
        if (points.Count > 0)
        {
            Dest = points[0];
        }

        // Sprite Stuff
        UnitSprites.Add("Walking", transform.Find("SpriteRighter/Sprite").gameObject);
        UnitSprites.Add("Shooting", transform.Find("SpriteRighter/Sprite_shooting").gameObject);
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
        if (beingControlled)
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
        // Debug.Log("Firing!");
        GameObject bulletInstance = GameObject.Instantiate(bulletPrefab, unitObj.transform.position, Quaternion.identity) as GameObject;
        bulletInstance.transform.LookAt(GetRandomAdjacentPosition(position, missRange));
        bulletInstance.GetComponent<Projectile>().Init(unitTeam, dmg);
    }

    // [PARAMS]: Vector3 targetPosition
    // For AI, pass this as target. th.targetsInRange[0].gameObject.transform.position
    public void AttemptShotAtPosition(Vector3 targetPosition)
    {
        if (canFire)
        {
            ValidFireAttempt(targetPosition);
        }
        else
        {
            InvalidFireAttempt();
        }
    }

    private void ValidFireAttempt(Vector3 targetPosition)
    {
        canFire = false;
        if (beingControlled)
        {
            unitPossessionHandler.PossessedUnitFired();
        }
        // Fire with perfect accuracy if controlled.
        float missRange = beingControlled ? CONTROLLED_MISS_RADIUS : MISS_RANGE_RADIUS;
        FireAtPosition(targetPosition, missRange);
    }

    private void InvalidFireAttempt()
    {
        if (canFireDelay == false)
        {
            canFireDelay = true;
            StartCoroutine(EnableFiring());
        }
    }

    public void AddTargetInRange(GameObject target)
    {
        th.targetsInRange.Add(target);
        ClearNullTargets();
        UpdateThreatState();
        if (UnitSprites.ContainsKey("Shooting"))
        {
            DisableAllSprites();
            UnitSprites["Shooting"].SetActive(true);
        }
    }

    public void UpdateThreatState()
    {
        threatState = DetermineThreatState(GetHighestThreatLevelInRange());
    }

    private int GetHighestThreatLevelInRange()
    {
        int highestThreat = -1;
        foreach (GameObject target in th.targetsInRange)
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

    public void DisableAllSprites()
    {
        foreach (string sprite in UnitSprites.Keys)
        {
            UnitSprites[sprite].SetActive(false);
        }
    }

    public void RemoveTargetInRange(GameObject target)
    {
        if (th.targetsInRange.Contains(target))
        {
            th.targetsInRange.Remove(target);
            ClearNullTargets();
        }
        if (th.targetsInRange.Count <= 0)
        {
            // Sprite = Walking
            if (UnitSprites.ContainsKey("Walking"))
            {
                DisableAllSprites();
                UnitSprites["Walking"].SetActive(true);
            }
        }
    }

    public void ClearNullTargets()
    {
        List<GameObject> toRemoveObjs = new List<GameObject>();
        foreach (GameObject obj in th.targetsInRange)
        {
            if (obj == null)
            {
                toRemoveObjs.Add(obj);
            }
        }
        foreach (GameObject markedUnit in toRemoveObjs)
        {
            th.targetsInRange.Remove(markedUnit);
        }
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
        points.Add(point);
        if (points.Count == 1)
        {
            Dest = points[0];
        }
    }

    public void RemovePoint(GameObject point)
    {
        points.Remove(point);
        removing = false;
        if (points.Count == 0)
        {
            Dest = null;
        }
        else
        {
            Dest = points[0];
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
        if (Dest != null)
        {
            // If an enemy is in range, stay still!
            // if (th.targetsInRange.Count > 0)
            if (threatState == "STAND" || threatState == "FLEE")
            {
                // Skirmish.
            }
            else
            {
                // Get movement direction.
                var newDest = new Vector3(Dest.transform.position.x, this.transform.position.y, Dest.transform.position.z);
                var heading = newDest - this.transform.position;
                var distance = heading.magnitude;
                var direction = heading / distance;

                float distToDest = Vector3.Distance(transform.position, Dest.transform.position);

                // If you are not close enough to your dest, keep moving towards it.
                if (distToDest >= MIN_DIST_TO_MOVEMENT_DEST)
                {
                    // Translate movement.
                    transform.Translate(direction * speed * Time.deltaTime);
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



    public void MovementUpdate()
    {
        if (beingControlled == false)
        {
            AIMovement();

            // If there's a valid target within range!
            if (th.targetsInRange.Count > 0)
            {

                ClearNullTargets();

                // Are there any targets left after the purge?
                if (th.targetsInRange.Count > 0)
                {
                    AttemptShotAtPosition(th.targetsInRange[0].gameObject.transform.position);
                }
            }
        }
        else if (beingControlled)
        {
            PossessedMovement();
        }
    }
}