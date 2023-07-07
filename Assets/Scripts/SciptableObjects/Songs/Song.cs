using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Song : ScriptableObject
{
    public AudioClip audioClip;
    public int beatsPerMinute;
    public int timeSignatureByFour;
}
