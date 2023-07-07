using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class WebGLCompatibility : MonoBehaviour
{
    AudioSource audioSource;
    bool isPaused = false;
    float pauseTime;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Play()
    {
        audioSource.time = 0f;
        audioSource.Play();
    }

    public void Pause()
    {
        audioSource.Pause();
        pauseTime = audioSource.time;
    }

    public void UnPause()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.time = pauseTime;
            audioSource.UnPause();
        }
    }
}
