using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Hero Sequencer/Hero Beat Action/Attack Action")]
public class HeroBeatAttackAction : HeroBeatAction
{
    public override void Act(ICharacterEvents characterEvents)
    {
        base.Act(characterEvents);

        characterEvents.OnAttack.Invoke();
    }
}
