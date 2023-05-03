using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PossessionHandler : MonoBehaviour
{
    public Text unitHP;
    public Text unitRoF;
    public Text unitRange;
    public Text unitTitle;
    Unit posUnit;

    public void setPossessed(Unit u)
    {
        posUnit = u;
    }

    // Update is called once per frame
    void Update()
    {
        if (posUnit != null)
        {
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
}
