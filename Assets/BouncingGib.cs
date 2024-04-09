using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingGib : MonoBehaviour
{
    [SerializeField]
    float lifetime = 0.75f;
    [SerializeField]
    float maxHoriDistance = 1.0f;

    float bezierHeight;

    SpriteRenderer spriteRenderer;

    Vector3 startingPoint;
    Vector3 endingPoint;
    Vector3 bezierPoint;
    float count = 0.0f;

    public BouncingGib(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    // Start is called before the first frame update
    void Start()
    {
        startingPoint = transform.position;
        endingPoint = GetEndPoint();
        CalculateBezierPoint();
    }

    void GenerateBezierHeight(float horizontalDist)
    {
        bezierHeight = 5 * Mathf.Sin(0.32f * horizontalDist + 1.5f);
    }

    Vector3 GetEndPoint()
    {
        Vector3 endPoint = new Vector3(startingPoint.x, startingPoint.y, startingPoint.z);
        float horizontalDist = Random.Range(-maxHoriDistance, maxHoriDistance);
        GenerateBezierHeight(horizontalDist);
        endPoint.x = endPoint.x + horizontalDist;
        return endPoint;
    }

    // Update is called once per frame
    void Update()
    {
        if (count < 1.0f)
        {
            count += 1.0f * Time.deltaTime;
            Vector3 m1 = Vector3.Lerp(startingPoint, bezierPoint, count);
            Vector3 m2 = Vector3.Lerp(bezierPoint, endingPoint, count);
            transform.position = Vector3.Lerp(m1, m2, count);
        }
        else
        {
            Destroy(gameObject);
        }
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
}
