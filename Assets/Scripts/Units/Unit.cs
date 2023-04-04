using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface Unit
{
    // Core unit stats
    public int hp { get; set;}
    public int dmg { get; set;}
    public int speed { get; set;}
    public float rof { get; set;}
    public int threatLevel { get; set;}

    public string team { get; set; }
    public bool beingControlled { get; set; }
    public GameObject unitObj { get; set; }
    public Vector3 controlDirection { get; set; }

    public void FireAtPosition(Vector3 position, float missRange);
    public void FireAtTransform(Transform trans);
    public void ControlledFire(Vector3 target);

    // Target Logic 
    public void AddTargetInRange(GameObject target);
    public void RemoveTargetInRange(GameObject target);
    public void ClearTargets();

    public int DealDamage(int damage);
    public void Initalize(List<GameObject> newObjs, string newTeam, float _fireDelay, float unitRange);

    // Point Logic
    public void RemovePoint(GameObject point);
    public void AddPoint(GameObject point);

    public Vector3 GetPositionNearPosition(Vector3 position, float randomness);
    public Vector3 GetPositionNearTransform(Transform trans, float randomness);
}