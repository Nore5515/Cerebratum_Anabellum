using UnityEngine;
using UnityEngine.SceneManagement;

class InputHandler
{
    RayHandler rayHandler;
    RayObj rayObj = new RayObj();

    public PathHandler pathHandler;

    PosHandler posHandler;

    public InputHandler(PosHandler _posHandler)
    {
        posHandler = _posHandler;
        pathHandler = posHandler.pathHandler;
        rayHandler = new RayHandler();
    }

    public void KeyChecks()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            posHandler.SetPossession(true);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            posHandler.FreePossession();
        }
    }

    public void UpdateFuncs()
    {
        rayObj = rayHandler.GenerateRayObj();
        posHandler.DrawLine(rayObj.hit.point);
        KeyChecks();
        HandleMouseInput();
    }

    public void HandleMouseInput()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            MouseHeldFuncs();
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            MouseUpFuncs();
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            MouseDownFuncs();
        }
    }

    void MouseUpFuncs()
    {
        if (pathHandler.pathDrawingMode)
        {
            pathHandler.StopDrawingPath();
        }
    }

    void MouseHeldFuncs()
    {
        pathHandler.AttemptFollowSphere(rayObj);
    }

    void MouseDownFuncs()
    {
        rayObj = rayHandler.GenerateRayObj();
        if (rayObj.hit.collider == null) return;

        if (posHandler.IsControlling())
        {
            posHandler.ControlledMouseDown(rayObj);
        }
        else
        {
            CommandModeMouseDown();
        }
    }

    void CommandModeMouseDown()
    {
        rayObj = rayHandler.GenerateLayeredRayObj("Spawner");
        if (rayObj.hit.collider != null)
        {
            HitSpawner();
            return;
        }
        rayObj = rayHandler.GenerateLayeredRayObj("Unit");
        if (rayObj.hit.collider != null)
        {
            HitUnit();
            return;
        }
    }

    void HitUnit()
    {
        Debug.Log("Hit unit!");
        posHandler.TryPossessUnit(rayObj.hit.collider.gameObject);
    }
    void HitSpawner()
    {
        pathHandler.HandleClickOnSpawner(rayObj);
    }
}