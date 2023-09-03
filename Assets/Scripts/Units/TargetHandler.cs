using System.Collections.Generic;
using UnityEngine;

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

