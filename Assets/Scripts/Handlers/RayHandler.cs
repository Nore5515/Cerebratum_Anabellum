using UnityEngine;

class RayHandler
{
    RaycastHit GetMouseToWorldHit(RayObj rayObj)
    {
        // int layerMask = 1 << 6;

        // layerMask |= (1 << 2);
        // layerMask = ~layerMask;
        // rayObj.ray = GenerateRayFromMouseInput();
        // Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask);
        RaycastHit caughtHit;
        Physics.Raycast(rayObj.ray, out caughtHit, Mathf.Infinity);
        return caughtHit;
    }

    RaycastHit GetHitAgainstLayer(RayObj rayObj, string maskName)
    {
        RaycastHit caughtHit;
        if (Physics.Raycast(rayObj.ray, out caughtHit, Mathf.Infinity, GenerateLayerMask(maskName)))
        {
            Debug.Log("Fired and hit a wall");
        }
        return caughtHit;
    }

    // if (Physics.Raycast(transform.position, transform.forward, 20.0f, mask))
    // {
    //     Debug.Log("Fired and hit a wall");
    // }

    public RayObj RayChecks(string layerMask)
    {
        RayObj rayObj = new RayObj();
        rayObj.ray = GenerateRayFromMouseInput();
        if (layerMask == "")
        {
            rayObj.hit = GetMouseToWorldHit(rayObj);
        }
        else
        {
            rayObj.hit = GetHitAgainstLayer(rayObj, layerMask);
        }
        return rayObj;
    }

    Ray GenerateRayFromMouseInput()
    {
        return Camera.main.ScreenPointToRay(Input.mousePosition);
    }

    LayerMask GenerateLayerMask(string maskName)
    {
        LayerMask mask = LayerMask.GetMask(maskName);
        return mask;
    }
}