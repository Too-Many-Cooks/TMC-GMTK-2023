using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class HeroHealthManager : MonoBehaviour
{
    [SerializeField]
    int health = 3;

    [SerializeField]
    int invincibilityDurationInBeats = 8;
    int currentInvincibilityDurationInBeats = 8;

    [SerializeField]
    HealthVisualization healthVisualizer;

    bool invincible = false;

    [SerializeField]
    MeshRenderer blinkingMeshRenderer;

    private void Start()
    {
        BeatManager.Instance.beatHitEvent.AddListener(OnBeatHit);
        healthVisualizer.UpdateHealth(health);
    }

    private void OnBeatHit(BeatManager.BeatEventData arg0)
    {
        if(!invincible)
        {
            return;
        }
        if(currentInvincibilityDurationInBeats <= 0)
        {
            invincible = false;
            return;
        }
        float duration = BeatManager.Instance.EstimatedTimeTillNextBeat;
        StartCoroutine(BlinkingCoroutine(duration));
        currentInvincibilityDurationInBeats--;
    }

    private IEnumerator BlinkingCoroutine(float duration)
    {
        int times = 2;
        for (int i = 0; i < times / 2; i++)
        {
            blinkingMeshRenderer.enabled = false;
            yield return new WaitForSeconds(duration / (float)times);
            blinkingMeshRenderer.enabled = true;
            yield return new WaitForSeconds(duration / (float)times);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnTriggerEnter2D(collision.collider);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (invincible)
            return;

        var damager = collision.gameObject.GetComponent<IDamager>();

        if (damager != null)
        {
            health--;
            healthVisualizer.UpdateHealth(health);
            if (health <= 0)
                HandleDeath();
            invincible = true;
            currentInvincibilityDurationInBeats = invincibilityDurationInBeats;
            //StartCoroutine(InvicibilityCoroutine(invincibilityDurationInBeats));
        }
    }

    private void HandleDeath()
    {
        // TODO: Maybe something cooler?
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator InvicibilityCoroutine(int duration)
    {
        invincible = true;

        yield return new WaitForSeconds(duration);
        invincible = false;
    }

}
