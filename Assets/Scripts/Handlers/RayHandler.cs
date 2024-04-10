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

    RaycastHit2D GetHit2DAgainstLayer(Vector3 vector3, string maskName)
    {
        RaycastHit2D caughtHit;
        caughtHit = Physics2D.Raycast(vector3, Vector2.zero, Mathf.Infinity, GenerateLayerMask(maskName));
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

    public RayObj2D GenerateLayered2DRayObj(string layerMask)
    {
        RayObj2D rayObj = new RayObj2D();
        Vector3 cameraVec = GenerateVector3FromMouseInput();
        rayObj.hit = GetHit2DAgainstLayer(cameraVec, layerMask);
        return rayObj;
    }

    // For 3D
    Ray GenerateRayFromMouseInput()
    {
        return Camera.main.ScreenPointToRay(Input.mousePosition);
    }

    // For 2D
    Vector3 GenerateVector3FromMouseInput()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    LayerMask GenerateLayerMask(string maskName)
    {
        LayerMask mask = LayerMask.GetMask(maskName);
        return mask;
    }
}