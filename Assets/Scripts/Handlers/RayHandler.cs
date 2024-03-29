using UnityEngine;

class RayHandler
{
    RaycastHit GetMouseToWorldHit(RayObj rayObj)
    {
        RaycastHit caughtHit;
        Physics.Raycast(rayObj.ray, out caughtHit, Mathf.Infinity);
        return caughtHit;
    }

    RaycastHit GetHitAgainstLayer(RayObj rayObj, string maskName)
    {
        RaycastHit caughtHit;
        Physics.Raycast(rayObj.ray, out caughtHit, Mathf.Infinity, GenerateLayerMask(maskName));
        return caughtHit;
    }

    public RayObj GenerateRayObj()
    {
        RayObj rayObj = new RayObj();
        rayObj.ray = GenerateRayFromMouseInput();
        rayObj.hit = GetMouseToWorldHit(rayObj);
        return rayObj;
    }

    public RayObj GenerateLayeredRayObj(string layerMask)
    {
        RayObj rayObj = new RayObj();
        rayObj.ray = GenerateRayFromMouseInput();
        rayObj.hit = GetHitAgainstLayer(rayObj, layerMask);
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