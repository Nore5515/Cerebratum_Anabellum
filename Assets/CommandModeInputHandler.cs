using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

// Command Mode refers to any time you are not actively possessing a unit.

// It is attached to the camera, within the camera obj.

public class CommandModeInputHandler : MonoBehaviour
{
    RayHandler rayHandler;
    RayObj rayObj = new RayObj();

    public PathHandler pathHandler;

    Tilemap tileMap;

    // Also referenced in InputHandler (PossessionInputHandler)
    public static bool commandLoopEnabled = false;

    public bool initCmdLoop = true;

    bool isDrawingPathFromSpawner = false;

    public bool spaceHeld = false;

    public PosHandler posHandler;

    // Start is called before the first frame update
    void Start()
    {
        rayHandler = new RayHandler();
        commandLoopEnabled = initCmdLoop;
    }

    // Update is called once per frame
    void Update()
    {
        if (commandLoopEnabled)
        {
            rayObj = rayHandler.GenerateRayObj();
            HandleKeyboardInput();
            HandleMouseInput();
        }
    }

    public void HandleKeyboardInput()
    {
        HandleEscapeHeld();
        HandleSpaceHeld();
    }

    void HandleEscapeHeld()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    void HandleSpaceHeld()
    {
        spaceHeld = Input.GetKey(KeyCode.Space);
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

    void MouseDownFuncs()
    {
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

        CommandModeMouseDown();
    }

    void CommandModeMouseDown()
    {
        if (RayCheckSpawnerDraw()) return;
        if (RayCheckSpawner()) return;
        if (RayCheckUnit()) return;
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
            HitSpawner(rayObj.hit.collider.gameObject);
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
        if (!spaceHeld) return;
        if (posHandler != null)
        {
            if (posHandler.TryPossessUnit(rayObj.hit.collider.gameObject))
            {
                commandLoopEnabled = false;
            }
        }
        else
        {
            Debug.LogError("No Possession Handler Assigned - CommandModeInputHandler");
        }
    }

    void HitDrawButton()
    {
        if (pathHandler != null)
        {
            pathHandler.HandleClickOnDrawButton(rayObj);
        }
        else
        {
            Debug.LogError("No Path Handler Assigned - CommandModeInputHandler");
        }
    }

    void HitSpawner(GameObject colliderObj)
    {
        if (pathHandler != null)
        {
            pathHandler.HandleClickOnSpawner(rayObj);
        }
        else
        {
            //if (colliderObj.GetComponent<SpawnerPathManager>() != null)
            //{
            //    pathHandler = colliderObj.GetComponent<SpawnerPathManager>();
            //    pathHandler.HandleClickOnSpawner(rayObj);
            //}
            Debug.LogError("No Path Handler Assigned - CommandModeInputHandler");
        }
    }
}
