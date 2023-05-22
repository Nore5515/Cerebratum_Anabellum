using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PossessionHandler
{

    public static PossessionHandler instance = new PossessionHandler();
    static CoroutineHandler coHand;

    public static Text unitHP;
    public static Text unitRoF;
    public static Text unitRange;
    public static Text unitTitle;
    public static Slider cooldownSlider;
    static Unit posUnit;

    static float unitMaxDelay;
    static float unitDelay;

    static bool coroutineRunning = false;

    public delegate void UpdateCalls();
    public static UpdateCalls uc;

    private static GameObject unitStats;

    public static void setUnitStatUI(GameObject gameObject){
        unitStats = gameObject;

        // TODO MAKE THIS MORE MODULAR LATEr
        unitHP = unitStats.transform.Find("UnitValues/UnitHP/UnitHP").gameObject.GetComponent<Text>();
        unitRoF = unitStats.transform.Find("UnitValues/UnitROF/UnitROF").gameObject.GetComponent<Text>();
        unitRange = unitStats.transform.Find("UnitValues/UnitRange/UnitRange").gameObject.GetComponent<Text>();
        unitTitle = unitStats.transform.Find("UnitValues/UnitTitle/UnitTitle").gameObject.GetComponent<Text>();
        cooldownSlider = unitStats.transform.Find("CooldownSlider").gameObject.GetComponent<Slider>();
        coHand = GameObject.Find("EnvComponents/CoroutineHandler").GetComponent<CoroutineHandler>();
        if (coHand is null)
        {
            Debug.Assert(coHand != null);
        }
    }

    public static void setPossessed(Unit u)
    {
        posUnit = u;
        if (posUnit != null)
        {
            Debug.Log(posUnit);
            Debug.Log(unitHP);
            unitHP.text = posUnit.hp.ToString();
            unitRoF.text = posUnit.rof.ToString();
            unitRange.text = posUnit.unitRange.ToString();
            unitTitle.text = posUnit.unitType;
        }
        else{
            unitHP.text = "NAN";
            unitRoF.text = "NAN";
            unitRange.text = "NAN";
            unitTitle.text = "NAN";
        }

    }

    // TODO: EVENT DELEGATE THIS
    public void PossessedUnitFired()
    {
        if (!coroutineRunning){
            unitMaxDelay = posUnit.rof * 0.5f;
            unitDelay = 0.0f;
            coroutineRunning = true;

            // This is, perhaps, oneo f the nastiest things ive ever done
            // TODO: Lord
            coHand.StartTimedUpdate(unitDelay, unitMaxDelay, TimedUpdate);
            // MonoBehaviour mb = GameObject.Find("Canvas").GetComponent<UI>();
            // mb.StartCoroutine(TimedUpdateRoutine());
        }
    }

    // Creates instance
    static PossessionHandler()
    {
        uc = () => TimedUpdate();
        unitMaxDelay = 0.0f;
        unitDelay = 0.0f;        
    }

    // Update is called once per frame
    static bool TimedUpdate()
    {
        Debug.Log(unitDelay);
        unitDelay += Time.deltaTime;
        cooldownSlider.value = unitDelay;
        if (unitDelay > unitMaxDelay){
            coroutineRunning = false;
            return false;
        }
        return true;
    }

    // IEnumerator TimedUpdateRoutine ()
	// {
	// 	while (unitDelay < unitMaxDelay) {
	// 		yield return new WaitForSeconds(0.05f);
	// 		TimedUpdate();
	// 	}
    //     coroutineRunning = false;
	// }


}