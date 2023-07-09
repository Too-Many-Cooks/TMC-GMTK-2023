using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour, IEnemy, IDamager
{
    float xOrigin;

    private void Start()
    {
        xOrigin = transform.localPosition.x;
    }

    private void LateUpdate()
    {
        Vector3 v = transform.localPosition;
        v.x = xOrigin;
        transform.localPosition = v;
    }

    public void Die()
    {
        //TODO: Do something cooler
        Destroy(gameObject);
    }
}
