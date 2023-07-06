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
        rayObj = rayHandler.RayChecks("");
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
        rayObj = rayHandler.RayChecks("");
        if (rayObj.hit.collider == null)
        {
            Debug.Log("NNo colliderfound.");
            return;
        }

        if (posHandler.IsControlling())
        {
            posHandler.ControlledMouseDown(rayObj);

        }
        else
        {
            rayObj = rayHandler.RayChecks("Spawner");
            if (rayObj.hit.collider == null) return;
            CommandModeMouseDown();
        }
    }

    void CommandModeMouseDown()
    {
        switch (rayObj.hit.collider.gameObject.tag)
        {
            case "spawner":
                pathHandler.HandleClickOnSpawner(rayObj);
                break;
            case "unit":
                Debug.Log("Hit unit!");
                posHandler.TryPossessUnit(rayObj.hit.collider.gameObject);
                break;
            default:
                break;
        }
    }
}