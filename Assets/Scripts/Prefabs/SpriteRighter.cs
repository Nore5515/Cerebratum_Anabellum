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

    public void Flip(){
        SpriteRenderer[] arr = this.gameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in arr)
        {
            sr.flipX = !sr.flipX;
        }
    }

    // Update is called once per frame
    void Update()
    {
        ProcessUnitState();
    }

    void ProcessUnitState()
    {
        if (hostUnit.AnimState == lastState)
        {
            if (lastKnownDirectionUp == IsUnitFacingUp())
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
        if (hostUnit.direction.x > 0 && hostUnit.direction.z > 0)
        {
            return true;
        }
        return false;
    }
}
