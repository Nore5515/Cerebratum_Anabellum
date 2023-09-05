using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretSpriteHandler : MonoBehaviour
{
    public bool flipped = false;
    public Unit hostUnit;

    string lastState = "";

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
        UpdateHori();

        StartCoroutine(SillySpin());
    }

    // Update is called once per frame
    void Update()
    {
        ProcessUnitState();
    }

    void ProcessUnitState()
    {
        if (!isSpriteAnimPlaying)
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
            isSpriteAnimPlaying = true;
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

    void EnforceWalkingState()
    {
        DisableAllSprites();
        EnableWalkingAnim();
    }

    void EnforceShootingState()
    {
        DisableAllSprites();
    }

    void EnforceIdleState()
    {
        DisableAllSprites();
    }

    void UpdateHori()
    {
        if (IsUnitFacingLeft())
        {
            FaceLeft();
        }
        else
        {
            FaceRight();
        }
    }

    void FaceLeft()
    {
        SpriteRenderer[] arr = gameObject.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer sr in arr)
        {
            sr.flipX = true;
        }
    }

    void FaceRight()
    {
        SpriteRenderer[] arr = gameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in arr)
        {
            sr.flipX = false;
        }
    }

    void EnableWalkingAnim()
    {
        if (IsUnitFacingUp())
        {
            FaceUp();
        }
        else
        {
            FaceDown();
        }
    }

    void FaceUp()
    {
    }

    void FaceDown()
    {
    }

    void DisableAllSprites()
    {
    }

    bool IsUnitFacingUp()
    {
        if (transform.TransformVector(hostUnit.direction).z > 0)
        {
            return true;
        }
        return false;
    }

    bool IsUnitFacingLeft()
    {
        if (transform.TransformVector(hostUnit.direction).x < 0)
        {
            return true;
        }
        return false;
    }
}
