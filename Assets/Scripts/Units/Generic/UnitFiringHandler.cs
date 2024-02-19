using UnityEngine;
using System.Collections;

public class UnitFiringHandler : MonoBehaviour
{
    public bool canFire { get; set; }
    public bool canFireDelay { get; set; }
    public bool firstFire { get; set; }   // Their first shot should be almost fully charged! 15% normal speed.

    float rof;
    GameObject bulletPrefab;
    string unitTeam;
    int dmg;

    public void Initialize(float _rof, GameObject _bulletPrefab, string _unitTeam, int _dmg)
    {
        firstFire = true;
        rof = _rof;
        bulletPrefab = _bulletPrefab;
        unitTeam = _unitTeam;
        dmg = _dmg;

        // TODO: See Unit.cs
        // I hate doing this. So annoying.
        dmg = Constants.INF_DMG;
    }

    // [PARAMS]: Vector3 targetPosition
    // For AI, pass this as target. unitTargetHandler.targetsInRange[0].gameObject.transform.position
    public void AttemptShotAtPosition(Vector3 targetPosition, bool beingControlled)
    {
        if (canFire)
        {
            ValidAIFireAttempt(targetPosition, beingControlled);
        }
        else
        {
            InvalidFireAttempt();
        }
    }

    public void ValidAIFireAttempt(Vector3 targetPosition, bool beingControlled)
    {
        canFire = false;
        // Fire with perfect accuracy if controlled.
        float missRange = Constants.MISS_RANGE_RADIUS;
        FireAtPosition(targetPosition, missRange);
    }

    public virtual void FireAtPosition(Vector3 position, float missRange)
    {
        GameObject bulletInstance = GenerateNewBulletPrefab();
        Vector3 zeroedTarget = position;
        zeroedTarget.z = Constants.ZED_OFFSET;
        bulletInstance.transform.LookAt(GetRandomAdjacentPosition(zeroedTarget, missRange));
        bulletInstance.GetComponent<Projectile>().SetProps(new Projectile.Props(unitTeam, dmg));
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

    GameObject GenerateNewBulletPrefab()
    {
        return Instantiate(bulletPrefab, transform.position, Quaternion.identity);
    }

    public void InvalidPosFireAttempt()
    {
        if (canFireDelay == false)
        {
            canFireDelay = true;
            StartCoroutine(EnableFiring());
        }
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

    public void ValidPosFireAttempt(Vector3 targetPosition)
    {
        canFire = false;
        if (canFireDelay == false)
        {
            canFireDelay = true;
            StartCoroutine(EnableFiring());
        }
        float missRange = Constants.CONTROLLED_MISS_RADIUS;
        //Vector3 spriteOffset = new(0.0f, 0.0f, 2.0f);
        Vector3 spriteOffset = new(0.0f, 0.0f, 0.0f);
        FireAtPosition(targetPosition + spriteOffset, missRange);
    }

    private void InvalidFireAttempt()
    {
        if (canFireDelay == false)
        {
            canFireDelay = true;
            StartCoroutine(EnableFiring());
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
        if (firstFire)
        {
            firstFire = false;
            return new WaitForSeconds(rof * Constants.FIRST_FIRE_DELAY);
        }
        else
        {
            return new WaitForSeconds(rof);
        }
    }

}
