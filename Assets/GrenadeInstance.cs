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
        Destroy(grenadeProjectileInstance);
        Destroy(gameObject);
    }

}
