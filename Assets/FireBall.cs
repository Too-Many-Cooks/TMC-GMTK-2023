using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FireBall : MonoBehaviour, IDamager
{
    [SerializeField]
    float velocity = 10f;

    private void Start()
    {
        //BeatManager.Instance.beatHitEvent.AddListener(OnBeatHit);
    }

    /*private void OnBeatHit(BeatManager.BeatEventData arg0)
    {
        if(arg0 =)
    }*/

    private void Update()
    {
        transform.position += -transform.right * velocity * Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnTriggerEnter2D(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<FireSkull>() == null)
            Destroy(gameObject);
    }
}
