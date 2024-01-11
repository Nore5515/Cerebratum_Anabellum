using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using static UnityEngine.RuleTile.TilingRuleOutput;

class PossessionInputHandler
{
    RayHandler rayHandler;
    RayObj rayObj = new RayObj();

    public PathHandler pathHandler;

    PosHandler posHandler;
    Tilemap tileMap;

    bool isDrawingPathFromSpawner = false;

    public PossessionInputHandler(PosHandler _posHandler, Tilemap tileMap)
    {
        posHandler = _posHandler;
        pathHandler = posHandler.pathHandler;
        rayHandler = new RayHandler();
        if (tileMap != null)
        {
            this.tileMap = tileMap;
        }
    }

    public void UpdateFuncs()
    {
        if (!CommandModeInputHandler.commandLoopEnabled)
        {
            rayObj = rayHandler.GenerateRayObj();
            posHandler.DrawLine(rayObj.hit.point);
            HandleKeyboardInput();
            HandleMouseInput();
        }
    }

    public void HandleKeyboardInput()
    {
        posHandler.GetPossessionMovement();
        HandleEscapeHeld();
        HandleLeftControlHeld();
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
        //isDrawingPathFromSpawner = false;

        //if (pathHandler.pathDrawingMode)
        //{
        //    pathHandler.StopDrawingPath();
        //}
    }

    void MouseHeldFuncs()
    {
    }

    Vector3 MousePositionZeroZed()
    {
        Vector3 zeroZed = new Vector3();
        Vector3 screenToWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        zeroZed.x = screenToWorldPos.x;
        zeroZed.y = screenToWorldPos.y;
        zeroZed.z = -0.5f;
        return zeroZed;
    }

    void MouseDownFuncs()
    {
        posHandler.PossessedMouseDown(rayObj);
    }
}