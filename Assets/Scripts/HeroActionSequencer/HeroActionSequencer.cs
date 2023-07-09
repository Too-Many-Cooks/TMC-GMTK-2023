using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static BeatManager;

[DefaultExecutionOrder(1)]
public class HeroActionSequencer : MonoBehaviour, ICharacterEvents
{
    private BeatManager beatManager;

    public ActionSequence HeroActionSequence { get => heroActionSequence; private set => heroActionSequence = value; }
    [SerializeField]
    ActionSequence heroActionSequence = null;

    [SerializeField]
    private int graceBars = 2;
    public int RemainingGraceBeats { get => remainingGraceBeats; private set => remainingGraceBeats = value; }
    [SerializeField]
    private int remainingGraceBeats = 0;
    
    public int CurrentActionSequencerIndex { get => currentActionSequencerIndex; private set => currentActionSequencerIndex = value; }
    int currentActionSequencerIndex = 0;

    [SerializeField] private UnityEvent<float> _onMove;
    public UnityEvent<float> OnMove { get { return _onMove; } }

    [SerializeField] private UnityEvent<float> _onMoveOver;
    public UnityEvent<float> OnMoveOver { get { return _onMoveOver; } }

    [SerializeField] private UnityEvent<bool> _onSprint;
    public UnityEvent<bool> OnSprint { get { return _onSprint; } }

    [SerializeField] private UnityEvent _onJump;
    public UnityEvent OnJump { get { return _onJump; } }

    private void Start()
    {
        beatManager = BeatManager.Instance;
        beatManager.beatHitEvent.AddListener(OnBeat);
        ResetSequencer();
    }

    public void ResetSequencer()
    {
        remainingGraceBeats = graceBars * beatManager.TimeSig;
        currentActionSequencerIndex = 0;
    }

    private void OnBeat(BeatEventData beatEventData)
    {
        if(remainingGraceBeats > 0)
        {
            remainingGraceBeats--;
            return;
        }

        if(heroActionSequence == null)
            return;

        var heroBeatAction = heroActionSequence.heroBeatActionSequence[currentActionSequencerIndex];
        heroBeatAction?.Act(this);
        currentActionSequencerIndex += 1;
        currentActionSequencerIndex %= heroActionSequence.heroBeatActionSequence.Length;
    }
}
