using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Static reference.

    public static AudioManager instance;

    void InitializeStaticReference()
    {
        if (instance == null)
            instance = this;
        else
        {
            Debug.LogError("There cannot be 2 simultaneous Audio Managers. Destroying this Audio Manager.");
            Destroy(this);
        }
    }

    #endregion


    public Sound[] sounds2D;

    // Attaches an Audio Source for every Sound and sets up the AudioSource values.
    // It also checks for name duplication in the Sound files.
    void InitializeSoundArray()
    {
        List<string> names = new List<string>();

        foreach (Sound sound in sounds2D)
        {            
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.audioClip;

            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;

            if (names.Contains(sound.name))
                Debug.LogError("Sound name '" + sound.name + "' is duplicated. " +
                    "This is not permitted and will cause errors when accessing the sound.");

            names.Add(sound.name);
        }
    }


    private void Awake()
    {
        InitializeStaticReference();

        InitializeSoundArray();
    }


    #region Sound Functions

    /// <summary>
    /// Plays and returns the sound that matches the given name in the sounds2D array. 
    /// If the sound is already playing, it restarts.
    /// </summary>
    /// <param name="clipName">Name of the sound in the sounds2D array.</param>
    public Sound PlaySound(string clipName)
    {
        Sound sound = FindSound(clipName);

        // If we are already playing, we Stop it.
        if (sound.source.isPlaying)
            sound.source.Stop();

        // We restart the clip and play it.
        sound.source.time = 0;
        sound.source.Play();

        return sound;
    }


    /// <summary>
    /// Plays and returns the sound that matches the given name in the sounds2D array. 
    /// If the sound is already playing, it restarts.
    /// </summary>
    /// <param name="clipName">Name of the sound in the sounds2D array.</param>
    /// <param name="loop">Sets the "Loop" var in the Audio Source to the selected value.</param>
    public Sound PlaySound(string clipName, bool loop)
    {
        Sound sound = FindSound(clipName);

        sound.source.loop = loop;

        // If we are already playing, we Stop it.
        if (sound.source.isPlaying)
            sound.source.Stop();

        // We restart the clip and play it.
        sound.source.time = 0;
        sound.source.Play();

        return sound;
    }


    /// <summary>
    /// Stops and returns the sound that matches the given name in the sounds2D array. 
    /// If the sound was not playing, it does nothing.
    /// </summary>
    /// <param name="clipName">Name of the sound in the sounds2D array.</param>
    public Sound StopSound(string clipName)
    {
        Sound sound = FindSound(clipName);

        if (sound.source.isPlaying)
            sound.source.Stop();

        return sound;
    }


    /// <summary>
    /// Pauses and returns the sound that matches the given name in the sounds2D array. 
    /// If the sound was not playing, it does nothing.
    /// </summary>
    /// <param name="clipName">Name of the sound in the sounds2D array.</param>
    public Sound PauseSound(string clipName)
    {
        Sound sound = FindSound(clipName);

        if (sound.source.isPlaying)
            sound.source.Pause();

        return sound;
    }


    /// <summary>
    /// Resumes and returns the sound that matches the given name in the sounds2D array. 
    /// If the sound was already playing, it does nothing.
    /// </summary>
    /// <param name="clipName">Name of the sound in the sounds2D array.</param>
    public Sound ResumeSound(string clipName)
    {
        Sound sound = FindSound(clipName);

        if (!sound.source.isPlaying)
            sound.source.UnPause();

        return sound;
    }


    /// <summary>
    /// Returns a sound that has the given name in the sounds2D array.
    /// </summary>
    /// <param name="clipName">Name of the sound in the sounds2D array.</param>
    /// <returns></returns>
    public Sound FindSound(string clipName)
    {
        // Finding the sound in our sound array.
        Sound s = Array.Find(sounds2D, sound => sound.name == clipName);

        // Not finding our sound.
        if (s == null)
        {
            Debug.LogWarning("Sound with name " + s.name + " could not be found. " +
                "Check if the sound has been added to the AudioManager sounds2D array.");
            return null;
        }

        return s;
    }

    #endregion
}
