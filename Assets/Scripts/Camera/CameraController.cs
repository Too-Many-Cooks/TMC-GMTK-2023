using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField][Range(0, 50)] float speedForApproachingHero = 10;


    [Header("References")]
    [SerializeField] Transform heroTransform;



    private void Update()
    {
        // We progressively approach the hero if we are not there.
        float deltaToHero = heroTransform.position.x - transform.position.x;

        if(Mathf.Abs(deltaToHero) < speedForApproachingHero * Time.deltaTime)
        {
            transform.localPosition += new Vector3(deltaToHero, 0, 0);
            return;
        }

        transform.localPosition += new Vector3(deltaToHero, 0, 0) * speedForApproachingHero * Time.deltaTime;
    }
}
