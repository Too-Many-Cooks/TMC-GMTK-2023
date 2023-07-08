using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DEBUG_StatWriter : MonoBehaviour
{
    public BeatManager beatManager;

    Text text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = beatManager.AudioSourcePlayTime + "\n" + beatManager.TotalBeatCounter;
    }
}
