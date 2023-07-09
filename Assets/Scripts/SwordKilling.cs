using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SwordKilling : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var enemy = other.gameObject.GetComponent<IEnemy>();

        if (enemy != null)
        {
            enemy.Die();
        }
    }

    public void SetKillboxActive(bool active)
    {
        GetComponent<Collider>().enabled = active;
    }
}
