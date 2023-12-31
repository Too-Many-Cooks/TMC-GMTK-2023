using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Hero Sequencer/Hero Beat Action/Move Action")]
public class HeroBeatMoveAction : HeroBeatAction
{
    public float desiredMove;
    public bool sprint = false;

    public override void Act(ICharacterEvents characterEvents)
    {
        base.Act(characterEvents);
        characterEvents.OnSprint.Invoke(sprint);
        characterEvents.OnMove.Invoke(desiredMove);
    }
}
