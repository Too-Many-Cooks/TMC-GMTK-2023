using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Attack Data")]
public class AttackData : ScriptableObject
{
    public bool isMelee = true;
    public float attackDuration;
    public float attackCooldown;
    public bool flipOnDirectionChange = true;
    public Rect meleeAttackCollider = Rect.zero;
    public AnimationClip animation;
}
