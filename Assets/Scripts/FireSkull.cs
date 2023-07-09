using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSkull : MonoBehaviour, IEnemy, IDamager
{
    [SerializeField]
    GameObject fireBallPrefab;

    bool first = true;

    public void Die()
    {
        //TODO: Make cooler
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        BeatManager.Instance.beatHitEvent.AddListener(OnBeatHit);
    }

    private void OnBeatHit(BeatManager.BeatEventData arg0)
    {
        if (first)
        {
            first = false;
            return;
        }
        if(arg0.beatNumberInTimeSig == 0)
        {
            FireFireBall();
        }
    }

    private void FireFireBall()
    {
        GameObject g = Instantiate(fireBallPrefab);
        g.transform.position = transform.position;
        g.transform.rotation = transform.rotation;
    }
}
