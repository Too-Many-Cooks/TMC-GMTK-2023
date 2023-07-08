using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static BeatManager;

public class HeroActionSequencer : MonoBehaviour
{
    [SerializeField]
    ActionSequence heroActionSequencer;
    int currentActionSequencerIndex = 0;

    [SerializeField]
    UnityEvent<HeroBeatAction> HeroBeatActionEvent;

    public void OnBeat(BeatEventData beatEventData)
    {
        if (heroActionSequencer.heroBeatActionSequence[currentActionSequencerIndex] != null)
        {
            HeroBeatActionEvent.Invoke(heroActionSequencer.heroBeatActionSequence[currentActionSequencerIndex]);
        }
        currentActionSequencerIndex += 1;
        currentActionSequencerIndex %= heroActionSequencer.heroBeatActionSequence.Length;
    }
}
