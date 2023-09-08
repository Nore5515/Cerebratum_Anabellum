using System.Collections;
using UnityEngine;

public class TurretSpriteHandler : MonoBehaviour
{
    public bool flipped = false;
    public Unit hostUnit;

    Sprite[] rotationSheet;
    Sprite[] activationSheet;

    int activationFrame = 0;
    int maxActivationFrame = 0;

    int spinFrame = 0;
    int maxSpinFrame = 0;

    bool isSpriteAnimPlaying = false;

    const float FRAME_RATE = 10.0f;

    //string spriteState = "Activation";
    string spriteState = "FaceDirection";

    TowerState towerState = TowerState.Idle;

    enum TowerState
    {
        Idle,
        Activating,
        Deactivating,
        RotatingTowards,
        FiringAt
    }

    enum Direction
    {
        E=0,
        SE=1,
        S=2,
        SW=3,
        W=4,
        NW=5,
        N=6,
        NE=7
    }

    Direction direction = Direction.E;

    void Start()
    {
        rotationSheet = Resources.LoadAll<Sprite>("Asset_TurretRotationSheet");
        activationSheet = Resources.LoadAll<Sprite>("Asset_TurretActivation");
        
        Debug.Log(rotationSheet.Length);

        maxActivationFrame = activationSheet.Length;
        maxSpinFrame = rotationSheet.Length;

        StartCoroutine(SillySpin());
    }

    // Update is called once per frame
    void Update()
    {
        ProcessUnitState();
    }

    void ProcessUnitState()
    {

        switch (hostUnit.AnimState)
        {
            case "Shooting":
                EnforceShootingState();
                break;
            case "Idle":
                EnforceIdleState();
                break;
            default:
                break;
        }

        if (!isSpriteAnimPlaying)
        {
            HandleTowerSpriteCommands();
            isSpriteAnimPlaying = true;
        }
    }

    void EnforceShootingState()
    {
        spriteState = "FaceDirection";
        direction = GetDirectionFromVector3(hostUnit.lastAimedTarget);
    }

    Direction GetDirectionFromVector3(Vector3 vector)
    {
        Vector3 dir = (transform.position - vector).normalized;
        if (dir.x > 0)
        {
            if (dir.z < 0.23f)
            {
                if (dir.z > 0.23f)
                {

                }
            }
        }
    }

    void EnforceIdleState()
    {
        if (towerState == TowerState.FiringAt || towerState == TowerState.RotatingTowards)
        {

        }
    }

    void HandleTowerSpriteCommands()
    {
        switch (spriteState)
        {
            case "Activation":
                StartCoroutine(IterateActivationAnim());
                break;
            case "Deactivation":
                StartCoroutine(IterateDeactivationAnim());
                break;
            case "Spin":
                StartCoroutine(IterateSpinAnim());
                break;
            case "Idle":
                GetComponent<SpriteRenderer>().sprite = activationSheet[0];
                break;
            case "FaceDirection":
                IterateFaceDirectionAnim(direction);
                break;
            default:
                break;
        }
    }

    IEnumerator SillySpin()
    {
        int dirSpin = 0;
        while (dirSpin < 8)
        {
            direction = (Direction)dirSpin;
            isSpriteAnimPlaying = false;
            dirSpin++;
            yield return new WaitForSeconds(1.0f / FRAME_RATE);
        }
        StartCoroutine(SillySpin());
    }

    void IterateFaceDirectionAnim(Direction dir)
    {
        GetComponent<SpriteRenderer>().sprite = rotationSheet[(int) dir];
    }

    IEnumerator IterateActivationAnim()
    {
        while (activationFrame < maxActivationFrame)
        {
            GetComponent<SpriteRenderer>().sprite = activationSheet[activationFrame];
            activationFrame++;
            yield return new WaitForSeconds(1.0f/FRAME_RATE);
        }
        activationFrame = 0;
        spriteState = "Spin";
        isSpriteAnimPlaying = false;
    }

    IEnumerator IterateDeactivationAnim()
    {
        activationFrame = maxActivationFrame - 1;
        while (activationFrame > 0)
        {
            GetComponent<SpriteRenderer>().sprite = activationSheet[activationFrame];
            activationFrame--;
            yield return new WaitForSeconds(1.0f / FRAME_RATE);
        }
        activationFrame = 0;
        spriteState = "Activation";
        isSpriteAnimPlaying = false;
    }

    IEnumerator IterateSpinAnim()
    {
        while (spinFrame < maxSpinFrame)
        {
            GetComponent<SpriteRenderer>().sprite = rotationSheet[spinFrame];
            spinFrame++;
            yield return new WaitForSeconds(1.0f/FRAME_RATE);
        }
        spinFrame = 0;
        spriteState = "Deactivation";
        isSpriteAnimPlaying = false;
    }
}
