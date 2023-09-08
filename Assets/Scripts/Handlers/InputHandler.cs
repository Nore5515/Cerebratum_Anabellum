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

    public void UpdateFuncs()
    {
        rayObj = rayHandler.GenerateRayObj();
        posHandler.DrawLine(rayObj.hit.point);
        HandleKeyboardInput();
        HandleMouseInput();
    }

    public void HandleKeyboardInput()
    {
        HandleEscapeHeld();
        HandleLeftControlHeld();
        HandleSpaceHeld();
    }

    void HandleEscapeHeld()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    void HandleLeftControlHeld()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            posHandler.FreePossession();
        }
    }

    void HandleSpaceHeld()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            posHandler.possessionKeyHeld = true;
        }
        else
        {
            posHandler.possessionKeyHeld = false;
        }
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
        rayObj = rayHandler.GenerateLayeredRayObj("Floor");
        pathHandler.AttemptPlaceSpawnerFollowObj(rayObj);
    }

    void MouseDownFuncs()
    {
        rayObj = rayHandler.GenerateLayeredRayObj("Floor");
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
        if (RayCheckSpawner()) return;
        if (RayCheckUnit()) return;
    }

    bool RayCheckSpawner()
    {
        rayObj = rayHandler.GenerateLayeredRayObj("Spawner");
        if (rayObj.hit.collider != null)
        {
            HitSpawner();
            return true;
        }
        return false;
    }

    bool RayCheckUnit()
    {
        rayObj = rayHandler.GenerateLayeredRayObj("Unit");
        if (rayObj.hit.collider != null)
        {
            HitUnit();
            return true;
        }
        return false;
    }

    void HitUnit()
    {
        posHandler.TryPossessUnit(rayObj.hit.collider.gameObject);
    }

    void HitSpawner()
    {
        pathHandler.HandleClickOnSpawner(rayObj);
    }
}