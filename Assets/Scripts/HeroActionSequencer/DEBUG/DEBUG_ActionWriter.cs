using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DEBUG_ActionWriter : MonoBehaviour
{
    public Text text;

    public void OnHeroAction(HeroBeatAction heroBeatAction)
    {
        text.text = "Hero action: " + heroBeatAction.Name;
    }
}
