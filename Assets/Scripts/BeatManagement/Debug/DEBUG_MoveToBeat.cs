using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEBUG_MoveToBeat : MonoBehaviour
{
    public Transform target;
    private Vector3 origin;
    public BeatManager beatManager;

    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float ratio = beatManager.EstimatedTimeTillNextBeat / beatManager.GetIntervalLength();
        transform.position = Vector3.Lerp(origin, target.position, 1f-ratio);
    }
}
