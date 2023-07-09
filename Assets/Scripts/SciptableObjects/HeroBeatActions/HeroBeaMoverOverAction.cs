using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Hero Sequencer/Hero Beat Action/Move Over Action")]
public class HeroBeatMoveOverAction : HeroBeatAction
{
    public float displacement;
    public bool sprint = false;

    public override void Act(ICharacterEvents characterEvents)
    {
        base.Act(characterEvents);
        characterEvents.OnSprint.Invoke(sprint);
        characterEvents.OnMoveOver.Invoke(displacement);
    }
}
