using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(WebGLCompatibility))]
public class BeatManager : MonoBehaviour
{
    public static BeatManager Instance;

    private WebGLCompatibility webGLCompatibility;

    [SerializeField]
    Song currentSong;

    //[SerializeField]
    AudioSource audioSource;

    float beatsPerMinute = 120f;

    public int TimeSig { get => timeSig; set => timeSig = value; }
    private int timeSig = 4;

    [SerializeField]
    public UnityEvent<BeatEventData> beatHitEvent;

    public float EstimatedTimeTillNextBeat { get => estimatedTimeTillNextBeat; set => estimatedTimeTillNextBeat = value; }
    private float estimatedTimeTillNextBeat;

    public float AudioSourcePlayTime { get => audioSourcePlayTime; set => audioSourcePlayTime = value; }
    private float audioSourcePlayTime;
    
    public int TotalBeatCounter { get => totalBeatCounter; set => totalBeatCounter = value; }
    
    private int totalBeatCounter;

    public struct BeatEventData
    {
        public int beatNumberInTimeSig;
        public float estimatedTimeTillNextBeat;

        public BeatEventData(int beatNumberInTimeSig, float estimatedTimeTillNextBeat)
        {
            this.beatNumberInTimeSig = beatNumberInTimeSig;
            this.estimatedTimeTillNextBeat = estimatedTimeTillNextBeat;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        audioSource = GetComponent<AudioSource>();
        webGLCompatibility = GetComponent<WebGLCompatibility>();
        LoadSong(currentSong);
        StartCoroutine(BeatTimingCoroutine());
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public void LoadSong(Song nextSong)
    {
        currentSong = nextSong;
        beatsPerMinute = nextSong.beatsPerMinute;
        timeSig = nextSong.timeSignatureByFour;
        audioSource.clip = currentSong.audioClip;
    }

    // Mostly based on this https://www.youtube.com/watch?v=gIjajeyjRfE
    private IEnumerator BeatTimingCoroutine()
    {
        yield return new WaitUntil(() => audioSource.isPlaying);
        int lastRegisteredBeatHit = -1;
        while(true)
        {
            // Calculates the elapsed time on the audio source (should be reliable)
            float elapsedTime = (float) audioSource.timeSamples/ (float) audioSource.clip.frequency;
            this.AudioSourcePlayTime = elapsedTime;
            // Divide it by a beat length
            int elapsedBeats = (int) (elapsedTime / (GetIntervalLength()));
            this.TotalBeatCounter = elapsedBeats;
            EstimatedTimeTillNextBeat = EstimateTimeTillNextBeat(elapsedTime, elapsedBeats);
            if(lastRegisteredBeatHit != elapsedBeats)
            {
                lastRegisteredBeatHit = elapsedBeats;
                beatHitEvent.Invoke(new BeatEventData(elapsedBeats % timeSig, EstimatedTimeTillNextBeat));
            }
            yield return null;
        }
    }



    private float EstimateTimeTillNextBeat(float elapsedTime, int elapsedBeats)
    {
        return GetIntervalLength() - (elapsedTime % GetIntervalLength());
    }

    public float GetIntervalLength()
    {
        return 60f / (beatsPerMinute);
    }
}
