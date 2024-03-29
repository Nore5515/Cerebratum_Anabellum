using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteRighter : MonoBehaviour
{
    public bool flipped = false;
    public Unit hostUnit;

    public GameObject walkingAnim;
    public GameObject walkingBackAnim;
    public GameObject firingAnim;
    public GameObject idleAnim;
    public GameObject idleBackAnim;
    public GameObject gunAnim;
    public GameObject gunBackAnim;

    public Text stateText;
    public Text stateText2;

    string lastState = "";

    void Start()
    {
        UpdateHori();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessUnitState();
    }

    void ProcessUnitState()
    {
        UpdateHori();

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
        if (IsUnitFacingUp())
        {
            if (idleBackAnim != null) { idleBackAnim.SetActive(true); }
            if (gunBackAnim != null) { gunBackAnim.SetActive(true); }
        }
        else
        {
            if (idleAnim != null) { idleAnim.SetActive(true); }
            if (gunAnim != null) { gunAnim.SetActive(true); }
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
    }

    void FaceLeft()
    {
        SpriteRenderer[] arr = gameObject.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer sr in arr)
        {
            sr.flipX = true;
        }
        if (stateText != null)
        {
            stateText2.text = "LEFT";
        }
    }

    void FaceRight()
    {
        SpriteRenderer[] arr = gameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in arr)
        {
            sr.flipX = false;
        }
        if (stateText != null)
        {
            stateText2.text = "RIGHT";
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
        if (walkingBackAnim != null) { walkingBackAnim.SetActive(true); }
        if (gunBackAnim != null) { gunBackAnim.SetActive(true); }
        if (stateText != null)
        {
            stateText.text = "UP";
        }
    }

    void FaceDown()
    {
        if (walkingAnim != null) { walkingAnim.SetActive(true); }
        if (gunAnim != null) { gunAnim.SetActive(true); }
        if (stateText != null)
        {
            stateText.text = "DOWN";
        }
    }

    void DisableAllSprites()
    {
        if (walkingAnim != null) { walkingAnim.SetActive(false); }
        if (walkingBackAnim != null) { walkingBackAnim.SetActive(false); }
        if (firingAnim != null) { firingAnim.SetActive(false); }
        if (idleAnim != null) { idleAnim.SetActive(false); }
        if (idleBackAnim != null) { idleBackAnim.SetActive(false); }
        if (gunAnim != null) { gunAnim.SetActive(false); }
        if (gunBackAnim != null) { gunBackAnim.SetActive(false); }
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
