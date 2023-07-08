using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class WebGLCompatibility : MonoBehaviour
{
    public bool autoplay = false;

    AudioSource audioSource;

    float pauseTime;

    public UnityEvent<bool> PauseStateChangedEvent;

    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (autoplay)
        {
            Play();
        }
    }

    private void Update()
    {
        
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
        PauseStateChangedEvent.Invoke(true);
    }

    public void UnPause()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.time = pauseTime;
            audioSource.UnPause();
            PauseStateChangedEvent.Invoke(false);
        }
    }
}

[CustomEditor(typeof(WebGLCompatibility))]
public class WebGLCompatibilityEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        WebGLCompatibility compatabilityLayer = (WebGLCompatibility)target;
        if (GUILayout.Button("Play"))
        {
            compatabilityLayer.Play();
        }
        if (GUILayout.Button("Pause"))
        {
            compatabilityLayer.Pause();
        }
        if (GUILayout.Button("Unpause"))
        {
            compatabilityLayer.UnPause();
        }
    }
}
