using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitPointHandler
{
    public Vector3 Dest;
    public List<GameObject> pointObjects = new();
    public List<Vector3> pointVectors = new();
    public bool removing = false;

    public void InitializePoints(List<GameObject> newPoints)
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

    public void AttemptRemoveNextDestPoint()
    {
        if (removing == false)
        {
            removing = true;
            RemovePoint(Dest);
        }
    }

    public void AddPoint(GameObject point)
    {
        pointVectors.Add(DuplicateVector(point.transform.position));
        if (pointVectors.Count == 1)
        {
            Dest = pointVectors[0];
        }
    }

    public void RemovePoint(GameObject point, Vector3 currentUnitPos)
    {
        pointVectors.Remove(point.transform.position);
        removing = false;
        if (pointVectors.Count == 0)
        {
            Dest = currentUnitPos;
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

    public void UpdatePoints(List<GameObject> newPoints)
    {
        foreach (GameObject newPoint in newPoints)
        {
            pointObjects.Add(newPoint);
            pointVectors.Add(newPoint.transform.position);
        }
    }
}

