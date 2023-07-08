using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Hero Sequencer/Hero Beat Action/Jump Action")]
public class HeroBeatJumpAction : HeroBeatAction
{
    public override void Act(ICharacterEvents characterEvents)
    {
        base.Act(characterEvents);

        characterEvents.OnJump.Invoke();
    }
}
