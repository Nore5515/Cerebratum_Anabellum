using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using static UnityEngine.RuleTile.TilingRuleOutput;

class PossessionInputHandler
{
    RayHandler rayHandler;
    RayObj rayObj = new RayObj();

    RaycastHit2D hit2D;

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

            hit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.zero);
            posHandler.DrawLine(hit2D.point);
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
        if (Input.GetKey(KeyCode.Mouse1))
        {
            AltMouseHeldFuncs();
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            MouseUpFuncs();
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            AltMouseUpFuncs();
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            MouseDownFuncs();
        }
    }

    void MouseUpFuncs()
    {
    }

    void AltMouseUpFuncs()
    {
        posHandler.ReleasedMouse();
    }

    void AltMouseHeldFuncs()
    {
        if (hit2D.point != null)
        {
            posHandler.HeldMouse(hit2D.point);
        }
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
        if (hit2D.point != null && posHandler != null)
        {
            posHandler.PossessedMouseDown(hit2D.point);
        }
    }
}