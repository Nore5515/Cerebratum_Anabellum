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

    string lastState = "";

    bool lastKnownDirectionUp = false;
    bool lastKnownDirectionLeft = false;

    public void Flip(){
        SpriteRenderer[] arr = this.gameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in arr)
        {
            sr.flipX = !sr.flipX;
        }
    }

    void FaceLeft()
    {
        SpriteRenderer[] arr = this.gameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in arr)
        {
            sr.flipX = true;
        }
    }

    void FaceRight()
    {
        SpriteRenderer[] arr = this.gameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in arr)
        {
            sr.flipX = false;
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
        if (hostUnit.AnimState == lastState)
        {
            if (lastKnownDirectionUp == IsUnitFacingUp() && lastKnownDirectionLeft == IsUnitFacingLeft())
            {
                return;
            }
        }
        if (hostUnit.AnimState == "Walking")
        {
            DisableAllSprites();
            EnableWalkingAnim();
        }
        if (hostUnit.AnimState == "Shooting")
        {
            DisableAllSprites();
            if (firingAnim != null) { firingAnim.SetActive(true); }
        }
        lastState = hostUnit.AnimState;
        lastKnownDirectionUp = IsUnitFacingUp();
        lastKnownDirectionLeft = IsUnitFacingLeft();
    }

    void CheckAndUpdateIfHoriChanged()
    {
        if (lastKnownDirectionLeft == IsUnitFacingLeft())
        {
            return;
        }
        else
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
        //if (hostUnit.direction.z > 0)
        //if (transform.TransformPoint(hostUnit.direction).x > 0 && transform.TransformPoint(hostUnit.direction).z > 0)
        if (transform.TransformPoint(hostUnit.direction).z > 0)
        {
            return true;
        }
        return false;
    }

    bool IsUnitFacingLeft()
    {
        //if (hostUnit.direction.x < 0)
        //if (transform.TransformPoint(hostUnit.direction).x < 0 && transform.TransformPoint(hostUnit.direction).z > 0)
        if (transform.TransformPoint(hostUnit.direction).x < 0)
        {
            return true;
        }
        return false;
    }
}
