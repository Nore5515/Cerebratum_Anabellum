using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitPointHandler
{
    public Vector3 DestVector;
    public List<Vector3> pointVectors = new();
    public bool removing = false;

    public void InitializePoints(List<GameObject> newPoints)
    {
        foreach (GameObject newPoint in newPoints)
        {
            pointVectors.Add(GetNearbyPoint(newPoint.transform.position, Constants.PATH_FOLLOW_DIVERGENCE));
        }
        if (pointVectors.Count > 0)
        {
            DestVector = pointVectors[0];
        }
    }

    public void AttemptRemoveNextDestPoint()
    {
        if (removing == false)
        {
            removing = true;
            RemovePoint(DestVector);
        }
    }

    public void AddPoint(GameObject point)
    {
        //pointVectors.Add(DuplicateVector(point.transform.position));
        pointVectors.Add(GetNearbyPoint(point.transform.position, Constants.PATH_FOLLOW_DIVERGENCE));
        if (pointVectors.Count == 1)
        {
            DestVector = pointVectors[0];
        }
    }

    private Vector3 GetNearbyPoint(Vector3 pointVec, float nearbyDist)
    {
        Vector3 nearbyPoint = DuplicateVector(pointVec);
        nearbyPoint.x += Random.Range(-nearbyDist, nearbyDist);
        nearbyPoint.z += Random.Range(-nearbyDist, nearbyDist);
        return nearbyPoint;
    }

    public void RemovePoint(GameObject point, Vector3 currentUnitPos)
    {
        pointVectors.Remove(point.transform.position);
        removing = false;
        if (pointVectors.Count == 0)
        {
            DestVector = currentUnitPos;
        }
        else
        {
            DestVector = pointVectors[0];
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
            DestVector = new Vector3(0.0f, 0.0f, 0.0f);
        }
        else
        {
            DestVector = pointVectors[0];
        }
    }

    public void UpdatePoints(List<GameObject> newPoints)
    {
        foreach (GameObject newPoint in newPoints)
        {
            pointVectors.Add(newPoint.transform.position);
        }
    }
}

