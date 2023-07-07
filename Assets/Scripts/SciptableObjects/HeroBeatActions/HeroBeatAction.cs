using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class HeroBeatAction : ScriptableObject
{
    public string Name;
    public string Description;
    public Sprite Icon;
    public bool InvolvesMovement;
    public bool InvolvesAttacking;
}
