using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField][Range(0, 50)] float speedForApproachingHero = 10;


    [Header("References")]
    [SerializeField] LevelMovementMeassurement myLevelMeassurementScript;
    


    private void Update()
    {
        // We progressively approach the hero if we are not there.
        Vector3 deltaToHero = -transform.localPosition;

        if(deltaToHero.magnitude < speedForApproachingHero * Time.deltaTime)
        {
            transform.localPosition = Vector3.zero;
            return;
        }

        transform.localPosition += deltaToHero.normalized * speedForApproachingHero * Time.deltaTime;
    }

    private void LateUpdate()
    {
        // We replicate the scenery movement.
        transform.localPosition += myLevelMeassurementScript.LevelMovementDelta;
    }
}
