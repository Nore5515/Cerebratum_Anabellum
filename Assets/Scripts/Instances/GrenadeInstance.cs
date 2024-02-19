using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Projectile;

public class GrenadeInstance : MonoBehaviour
{
    public float grenadeLifetime = 1.0f;
    Vector3 playerPos;

    Vector3 startingPoint;
    Vector3 endingPoint;
    Vector3 bezierPoint;

    [SerializeField]
    float bezierHeight = 5.0f;

    [SerializeField]
    GameObject grenadeProjectilePrefab;

    GameObject grenadeProjectileInstance;

    bool released = false;
    float count = 0.0f;

    Vector3 detonationExtents;

    private void Start()
    {
        detonationExtents = new Vector3(Constants.GRENADE_LOCAL_SCALE, Constants.GRENADE_LOCAL_SCALE * 0.5f, Constants.GRENADE_LOCAL_SCALE);
    }

    void CalculateBezierPoint()
    {
        float bezierX = startingPoint.x + ((endingPoint.x - startingPoint.x) / 2);
        float bezierY = startingPoint.y + ((endingPoint.y - startingPoint.y) / 2);
        float bezierZ = startingPoint.z;
        bezierPoint = new Vector3(bezierX, bezierY, bezierZ);
        // At this point, bezier Point is a halfway point between starting and ending point.
        // We now add some UP, to get that nice curve.
        bezierPoint += Vector3.up * bezierHeight;
    }

    public void ReleasedFromPlayer(Vector3 playerPos)
    {
        if (!released)
        {
            released = true;
            startingPoint = playerPos;
            endingPoint = transform.position;
            IEnumerator coroutine = BeginDetonationSequence();
            StartCoroutine(coroutine);
            CalculateBezierPoint();
        }
    }

    private void Update()
    {
        if (released)
        {
            if (grenadeProjectileInstance == null)
            {
                grenadeProjectileInstance = Instantiate(grenadeProjectilePrefab, startingPoint, transform.rotation);
            }
            else
            {
                if (count < 1.0f)
                {
                    count += 1.0f * Time.deltaTime;
                    Vector3 m1 = Vector3.Lerp(startingPoint, bezierPoint, count);
                    Vector3 m2 = Vector3.Lerp(bezierPoint, endingPoint, count);
                    grenadeProjectileInstance.transform.position = Vector3.Lerp(m1, m2, count);
                }
                else
                {
                    Debug.Log("NO NO NO NO");
                }
            }
        }
    }

    /// <summary>
    /// Destroys self.
    /// </summary>
    IEnumerator BeginDetonationSequence()
    {
        yield return new WaitForSeconds(grenadeLifetime);
        DamageEnemies();
        Destroy(grenadeProjectileInstance);
        Destroy(gameObject);
    }

    void DamageEnemies()
    {
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, detonationExtents / 2, Quaternion.identity);

        foreach (Collider c in hitColliders)
        {
            Debug.Log(c.name);
            Unit u = GetUnitFromCollider(c);
            if (u != null)
            {
                u.ReceiveDamage(1);
            }
        }
    }

    Unit GetUnitFromCollider(Collider c)
    {
        if (c.name == "UnitCollisionDetector")
        {
            Unit detectedUnit = c.transform.parent.gameObject.GetComponent<Unit>();
            if (detectedUnit != null)
            {
                return detectedUnit;
            }
        }
        return null;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
        Gizmos.DrawWireCube(transform.position, detonationExtents);
    }
}
