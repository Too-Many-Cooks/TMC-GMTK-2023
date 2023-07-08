using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Hero Sequencer/Hero Beat Action/No Action")]
public class HeroBeatAction : ScriptableObject
{
    public string Name;
    public string Description;
    public Sprite Icon;

    public virtual void Act(ICharacterEvents characterEvents)
    {

    }
}
