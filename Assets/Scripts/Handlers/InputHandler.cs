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
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //RaycastHit caughtHit;
        //Physics.Raycast(ray, out caughtHit, Mathf.Infinity, LayerMask.GetMask("Spawner"));
        //if (caughtHit.collider != null)
        //{
        //    Debug.Log(caughtHit.collider.name);
        //}
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
        //rayObj = rayHandler.GenerateLayeredRayObj("Floor");
        //if (rayObj.hit.collider != null)
        //{
        //    Debug.Log(rayObj.hit.collider.name);
        //}
        //else
        //{
        //    //Debug.Log("null");
        //}
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
        isDrawingPathFromSpawner = false;

        if (pathHandler.pathDrawingMode)
        {
            pathHandler.StopDrawingPath();
        }
    }

    void MouseHeldFuncs()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null)
        {
            if (isDrawingPathFromSpawner)
            {
                pathHandler.MouseHeldAndDraggedAtPosition(MousePositionZeroZed());
            }
        }
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
        //Debug.Log("Mouse down!");
        // TODO: Im drunk and a little tired; look at tomorrow when sober
        //rayObj = rayHandler.GenerateLayeredRayObj("Floor");
        //if (rayObj.hit.collider == null) return;



        // RaycastHit hit = Physics.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, 9);
        RaycastHit hit = rayHandler.GenerateLayeredRayObj("SpawnerUI").hit;

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.name == "SpawnerPathButton")
            {
                isDrawingPathFromSpawner = true;

                Spawner spawnerClass = pathHandler.spawnerSource.GetComponent<Spawner>();
                spawnerClass.spawnerPathManager.ClearPoints(spawnerClass.unitList);
            }
        }

        //if (posHandler.IsControlling())
        //{
        posHandler.ControlledMouseDown(rayObj);
        //}
        //else
        //{
        //    CommandModeMouseDown();
        //}
    }

    void CommandModeMouseDown()
    {
        if (RayCheckSpawnerDraw()) return;
        if (RayCheckSpawner()) return;
        if (RayCheckUnit()) return;
    }

    bool RayCheckSpawnerDraw()
    {
        rayObj = rayHandler.GenerateLayeredRayObj("SpawnerUI");
        if (rayObj.hit.collider != null)
        {
            Debug.Log("Hit button!");
            HitDrawButton();
            return true;
        }
        return false;
    }

    bool RayCheckSpawner()
    {
        rayObj = rayHandler.GenerateLayeredRayObj("Spawner");
        Debug.Log("Spawner?");
        if (rayObj.hit.collider != null)
        {
            Debug.Log("Hit spawner!");
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
            HitUnit(rayObj);
            return true;
        }
        return false;
    }

    void HitUnit(RayObj rayObj)
    {
        posHandler.TryPossessUnit(rayObj.hit.collider.gameObject);
    }

    void HitDrawButton()
    {
        pathHandler.HandleClickOnDrawButton(rayObj);
    }

    void HitSpawner()
    {
        pathHandler.HandleClickOnSpawner(rayObj);
    }
}