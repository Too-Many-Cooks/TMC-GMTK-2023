using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Hero Sequencer/Action Sequence")]
public class ActionSequence : ScriptableObject
{
    public HeroBeatAction[] heroBeatActionSequence;
}
