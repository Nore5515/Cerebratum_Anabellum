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

    public RayObj RayChecks()
    {
        RayObj rayObj = new RayObj();
        rayObj.ray = GenerateRayFromMouseInput();
        rayObj.hit = GetMouseToWorldHit(rayObj);
        return rayObj;
    }

    Ray GenerateRayFromMouseInput()
    {
        return Camera.main.ScreenPointToRay(Input.mousePosition);
    }

    LayerMask GenerateLayerMask(string maskName)
    {
        LayerMask mask = LayerMask.GetMask("Wall");

        // // Check if a Wall is hit.
        // if (Physics.Raycast(transform.position, transform.forward, 20.0f, mask))
        // {
        //     Debug.Log("Fired and hit a wall");
        // }
        return mask;
    }
}