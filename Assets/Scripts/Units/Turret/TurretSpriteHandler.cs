using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

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
    string lastSpriteState = "";
    string spriteState = "FaceDirection";

    TowerState towerState = TowerState.Idle;

    public bool verboseLogs = false;

    public TMPro.TextMeshProUGUI angleText;

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
        }

        lastSpriteState = spriteState;
    }

    void EnforceShootingState()
    {
        //if (spriteState == "Idle" && !isSpriteAnimPlaying)
        //{
        //    spriteState = "Activation";
        //}
        //else if (spriteState == "Face Direction" && !isSpriteAnimPlaying)
        //{

        //}
        spriteState = "FaceDirection";
        direction = GetDirectionFromVector3(hostUnit.lastAimedTarget);
    }

    Direction GetDirectionFromVector3(Vector3 vector)
    {
        Vector3 dir = (transform.position - vector);

        float angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;

        if (angleText != null) {
            angleText.text = angle.ToString();
        }
        //angle -= 180f;

        //if (angle < 0)
        //{
        //angle += 360;
        //}
        Debug.Log(angle);
        if (angle < 0)
        {
            return Direction.S;
        }

        //angle = 0;

        if (Input.GetKey(KeyCode.Alpha1)) angle = 45;
        if (Input.GetKey(KeyCode.Alpha2)) angle = 90;
        if (Input.GetKey(KeyCode.Alpha3)) angle = 135;
        if (Input.GetKey(KeyCode.Alpha4)) angle = 180;
        if (Input.GetKey(KeyCode.Alpha5)) angle = 225;
        if (Input.GetKey(KeyCode.Alpha6)) angle = 270;
        if (Input.GetKey(KeyCode.Alpha7)) angle = 315;
        if (Input.GetKey(KeyCode.Alpha8)) angle = 360;

        int spriteIndex = Mathf.RoundToInt(angle / 45f) % 8;

        return (Direction)spriteIndex;
    }

    void EnforceIdleState()
    {
        if (spriteState == "Waiting" && !isSpriteAnimPlaying)
        {
            spriteState = "FaceDirection";
            direction = Direction.E;
        }
        else if (spriteState == "FaceDirection" && !isSpriteAnimPlaying)
        {
            spriteState = "Deactivation";
        }
        else
        {
            spriteState = "Idle";
        }
    }

    void HandleTowerSpriteCommands()
    {
        switch (spriteState)
        {
            case "Activation":
                WakeupSequenceAnim();
                break;
            case "Deactivation":
                ShutdownSequenceAnim();
                break;
            case "Spin":
                StartCoroutine(IterateSpriteAnimAscending("Waiting", rotationSheet));
                break;
            case "Idle":
                GetComponent<SpriteRenderer>().sprite = activationSheet[0];
                isSpriteAnimPlaying = false;
                break;
            case "Waiting":
                GetComponent<SpriteRenderer>().sprite = rotationSheet[0];
                isSpriteAnimPlaying = false;
                break;
            case "FaceDirection":
                IterateFaceDirectionAnim(direction);
                break;
            default:
                break;
        }
    }

    void ShutdownSequenceAnim()
    {
        StartCoroutine(IterateSpriteAnimDescending("Idle", activationSheet));
    }

    void WakeupSequenceAnim()
    {
        StartCoroutine(IterateSpriteAnimAscending("Waiting", activationSheet));
    }

    void IterateFaceDirectionAnim(Direction dir)
    {
        isSpriteAnimPlaying = true;
        //Debug.Log(dir);
        GetComponent<SpriteRenderer>().sprite = rotationSheet[(int) dir];
        isSpriteAnimPlaying = false;
    }

    IEnumerator IterateSpriteAnimAscending(string completionSpriteState, Sprite[] spriteSheet)
    {
        isSpriteAnimPlaying = true;
        int currentFrame = 0;
        while (currentFrame < spriteSheet.Length)
        {
            GetComponent<SpriteRenderer>().sprite = spriteSheet[currentFrame];
            currentFrame++;
            yield return new WaitForSeconds(1.0f / FRAME_RATE);
        }
        spriteState = completionSpriteState;
        isSpriteAnimPlaying = false;
    }

    IEnumerator IterateSpriteAnimDescending(string completionSpriteState, Sprite[] spriteSheet)
    {
        isSpriteAnimPlaying = true;
        int currentFrame = spriteSheet.Length - 1;
        while (currentFrame > 0)
        {
            GetComponent<SpriteRenderer>().sprite = spriteSheet[currentFrame];
            currentFrame--;
            yield return new WaitForSeconds(1.0f / FRAME_RATE);
        }
        spriteState = completionSpriteState;
        isSpriteAnimPlaying = false;
    }

    
}
