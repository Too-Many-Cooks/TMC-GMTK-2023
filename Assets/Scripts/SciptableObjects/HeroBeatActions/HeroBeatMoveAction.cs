using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Hero Sequencer/Hero Beat Action/Move Action")]
public class HeroBeatMoveAction : HeroBeatAction
{
    public float desiredMove;

    public override void Act(ICharacterEvents characterEvents)
    {
        base.Act(characterEvents);

        characterEvents.OnMove.Invoke(desiredMove);
    }
}
