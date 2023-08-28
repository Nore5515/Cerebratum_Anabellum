using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRighter : MonoBehaviour
{
    Quaternion rot = new Quaternion(0.0f, 0.0f, 0.0f, 1);
    public bool flipped = false;
    public Unit hostUnit;

    public GameObject walkingAnim;
    public GameObject walkingBackAnim;
    public GameObject firingAnim;
    public GameObject idleAnim;

    string lastState = "";

    bool lastKnownDirectionUp;
    bool lastKnownDirectionLeft;

    void Start()
    {
        UpdateHori();
        lastKnownDirectionUp = IsUnitFacingUp();
    }


    void FaceLeft()
    {
        SpriteRenderer[] arr = this.gameObject.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer sr in arr)
        {
            sr.flipX = true;
            //sr.transform.localScale = new Vector3(10.0f, 10.0f, 10.0f);
        }
    }

    void FaceRight()
    {
        SpriteRenderer[] arr = this.gameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in arr)
        {
            sr.flipX = false;
            //sr.transform.localScale = startingLocalScale;
        }
    }

    // Update is called once per frame
    void Update()
    {
        ProcessUnitState();
    }

    void ProcessUnitState()
    {
        CheckAndUpdateIfHoriChanged();

        if (hostUnit.AnimState == lastState) return;

        switch (hostUnit.AnimState)
        {
            case "Walking":
                EnforceWalkingState();
                break;
            case "Shooting":
                EnforceShootingState();
                break;
            case "Idle":
                EnforceIdleState();
                break;
            default:
                break;
        }

        lastState = hostUnit.AnimState;
    }

    void EnforceWalkingState()
    {
        DisableAllSprites();
        EnableWalkingAnim();
    }

    void EnforceShootingState()
    {
        DisableAllSprites();
        if (firingAnim != null) { firingAnim.SetActive(true); }
    }

    void EnforceIdleState()
    {
        DisableAllSprites();
        if (idleAnim != null) { idleAnim.SetActive(true); }
    }

    void CheckAndUpdateIfHoriChanged()
    {
        if (lastKnownDirectionLeft == IsUnitFacingLeft())
        {
            return;
        }
        else
        {
            UpdateHori();
        }
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
        lastKnownDirectionLeft = IsUnitFacingLeft();
    }

    void EnableWalkingAnim()
    {
        if (IsUnitFacingUp())
        {
            if (walkingBackAnim != null) { walkingBackAnim.SetActive(true); }
        }
        else
        {
            if (walkingAnim != null) { walkingAnim.SetActive(true); }
            //if (walkingBackAnim != null) { walkingBackAnim.SetActive(true); }
        }
    }

    void DisableAllSprites()
    {
        if (walkingAnim != null) { walkingAnim.SetActive(false); }
        if (walkingBackAnim != null) { walkingBackAnim.SetActive(false); }
        if (firingAnim != null) { firingAnim.SetActive(false); }
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
