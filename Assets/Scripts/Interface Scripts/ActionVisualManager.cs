using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(2)]
[RequireComponent(typeof(ActionHitEffect))]
public class ActionVisualManager : MonoBehaviour
{
    ActionHitEffect actionHitEffect;
    //List<GameObject> actionVisualQueue = new List<GameObject>();

    [SerializeField]
    GameObject actionInterfacePrefab;

    [SerializeField]
    HeroActionSequencer heroActionSequencer;

    [SerializeField]
    private int lookAheadBeats = 4;
    float beatHitEffectCooldown = 0f;

    private void Awake()
    {
        
        actionHitEffect = GetComponent<ActionHitEffect>();
    }

    // Start is called before the first frame update
    void Start()
    {
        BeatManager.Instance.beatHitEvent.AddListener(OnBeatHit);
    }

    private void OnBeatHit(BeatManager.BeatEventData beatEventData)
    {
        if (lookAheadBeats <= heroActionSequencer.RemainingGraceBeats)
            return;
        
        int lookAheadIndex = (heroActionSequencer.CurrentActionSequencerIndex + lookAheadBeats - heroActionSequencer.RemainingGraceBeats -1) 
            % heroActionSequencer.HeroActionSequence.heroBeatActionSequence.Length;
        //Debug.Log(lookAheadIndex);
        // If its an empty beat -> do nothing
        if (heroActionSequencer.HeroActionSequence.heroBeatActionSequence[lookAheadIndex] == null)
            return;
        
        float timeTillLookAheadBeat = BeatManager.Instance.GetIntervalLength() * (lookAheadBeats - 1) + beatEventData.estimatedTimeTillNextBeat;

        AddActionToQueue(heroActionSequencer.HeroActionSequence.heroBeatActionSequence[lookAheadIndex], timeTillLookAheadBeat);
    }

    // Update is called once per frame
    void Update()
    {
        if(beatHitEffectCooldown > 0f)
        {
            beatHitEffectCooldown -= Time.deltaTime;
            return;
        }
        if (BeatManager.Instance.EstimatedTimeTillNextBeat == 0f)
            return;

        if(BeatManager.Instance.EstimatedTimeTillNextBeat < actionHitEffect.growingScaleTimeLength)
        {
            beatHitEffectCooldown = actionHitEffect.growingScaleTimeLength * 2f;
            actionHitEffect.ActivateHitEffect();
        }
    }



    public void AddActionToQueue(HeroBeatAction heroBeatAction, float estimatedTimeTillBeatHit)
    {
        GameObject newActionInterface = Instantiate(actionInterfacePrefab, transform);
        newActionInterface.GetComponent<Image>().sprite = heroBeatAction.Icon;
        newActionInterface.GetComponent<ActionInterfaceBehavior>().DurationOfTimer = estimatedTimeTillBeatHit;
        //actionVisualQueue.Add(newActionInterface);
    }
}
