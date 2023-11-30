using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

class InputHandler
{
    RayHandler rayHandler;
    RayObj rayObj = new RayObj();

    public PathHandler pathHandler;

    PosHandler posHandler;
    Tilemap tileMap;

    public InputHandler(PosHandler _posHandler, Tilemap tileMap)
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
        rayObj = rayHandler.GenerateLayeredRayObj("Floor");
        if (rayObj.hit.collider != null)
        {
            Debug.Log(rayObj.hit.collider.name);
        }
        else
        {
            //Debug.Log("null");
        }
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
        pathHandler.MouseHeldAndDraggedAtPosition(MousePositionZeroZed());
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
            HitUnit();
            return true;
        }
        return false;
    }

    void HitUnit()
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