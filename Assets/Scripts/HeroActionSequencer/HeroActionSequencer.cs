using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static BeatManager;

public class HeroActionSequencer : MonoBehaviour, ICharacterEvents
{
    [SerializeField]
    ActionSequence heroActionSequencer = null;
    int currentActionSequencerIndex = 0;

    [SerializeField] private UnityEvent<float> _onMove;
    public UnityEvent<float> OnMove { get { return _onMove; } }

    [SerializeField] private UnityEvent _onJump;
    public UnityEvent OnJump { get { return _onJump; } }

    public void OnBeat(BeatEventData beatEventData)
    {
        if(heroActionSequencer == null)
            return;

        var heroBeatAction = heroActionSequencer.heroBeatActionSequence[currentActionSequencerIndex];
        heroBeatAction?.Act(this);
        currentActionSequencerIndex += 1;
        currentActionSequencerIndex %= heroActionSequencer.heroBeatActionSequence.Length;
    }
}
