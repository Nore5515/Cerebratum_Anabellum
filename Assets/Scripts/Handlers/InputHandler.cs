using UnityEngine;
using UnityEngine.SceneManagement;

class InputHandler
{
    public void KeyChecks()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            SetPossession(true);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            FreePossession();
        }
    }

    public void SetPossession(bool newPossession)
    {
        possessionReady = newPossession;
        possessionButton.GetComponent<Button>().interactable = !possessionReady;
    }


    void HandleMouseInput()
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
        if (pathDrawingMode)
        {
            StopDrawingPath();
        }
    }

    void MouseHeldFuncs()
    {
        if (spawnerSource == null) return;
        if (!pathDrawingMode) return;
        if (rayObj.hit.collider == null) return;
        if (rayObj.hit.collider.gameObject.tag == "floor")
        {
            TryPlaceFollowSphere();
        }
    }

    void MouseDownFuncs()
    {
        rayObj = rayHandler.RayChecks();
        if (rayObj.hit.collider == null) return;

        if (IsControlling())
        {
            controlledUnits[0].AttemptShotAtPosition(new Vector3(rayObj.hit.point.x, 0.5f, rayObj.hit.point.z));
        }
        else
        {
            CommandModeMouseDown();
        }
    }

    void CommandModeMouseDown()
    {
        switch (rayObj.hit.collider.gameObject.tag)
        {
            case "spawner":
                HandleClickOnSpawner();
                break;
            case "unit":
                Debug.Log("Hit unit!");
                TryPossessUnit(rayObj.hit.collider.gameObject);
                break;
            default:
                break;
        }
    }
}