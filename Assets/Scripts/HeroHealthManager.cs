using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(1)]
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
    Renderer blinikingRenderer;

    [SerializeField]
    float crushIntersectionTreshold = 0.3f;
    [SerializeField]
    int framesPerCrushDamage = 10;
    int fixedFrameCount = 0;
    int lastCrushFrame = int.MinValue;
    int crushFrameCount = 0;


    #region Audio

    AudioManager _myAudioManager;
    AudioManager MyAudioManager
    {
        get
        {
            if (_myAudioManager == null)
                _myAudioManager = AudioManager.instance;

            return _myAudioManager;
        }
    }

    const string takeDamageSoundName = "HeroHit";

    #endregion


    private void Start()
    {
        healthVisualizer = HealthVisualization.Instance;
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
            blinikingRenderer.enabled = false;
            yield return new WaitForSeconds(duration / (float)times);
            blinikingRenderer.enabled = true;
            yield return new WaitForSeconds(duration / (float)times);
        }
    }

    private void FixedUpdate()
    {
        fixedFrameCount++;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnTriggerEnter2D(collision.collider);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.layer != LayerMask.NameToLayer("Enemy"))
        {
            foreach (var contact in collision.contacts)
            {
                if (contact.separation < -crushIntersectionTreshold)
                {
                    ContactFilter2D contactFilter = new ContactFilter2D();
                    contactFilter.NoFilter();
                    contactFilter.useTriggers = false;
                    string[] layers = { "Default" };
                    contactFilter.layerMask.value = LayerMask.GetMask(layers);
                    contactFilter.useLayerMask = true;
                    List<RaycastHit2D> hits = new List<RaycastHit2D>();
                    if (collision.otherRigidbody.Cast(contact.normal, contactFilter, hits, Mathf.Abs(contact.separation)) > 1)
                    {
                        foreach (var hit in hits)
                        {
                            if (hit.collider != contact.collider && Vector2.Angle(contact.normal, hit.normal) > 90)
                            {
                                if(lastCrushFrame == fixedFrameCount)
                                    return;
                                if(lastCrushFrame == fixedFrameCount - 1)
                                {
                                    crushFrameCount++;
                                } else
                                {
                                    crushFrameCount = 0;
                                }
                                if(crushFrameCount > framesPerCrushDamage)
                                {
                                    Damage();
                                    crushFrameCount = 0;
                                }
                                lastCrushFrame = fixedFrameCount;
                            }
                        }
                    }
                }
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (invincible)
            return;

        var damager = collision.gameObject.GetComponent<IDamager>();

        if (damager != null)
        {
            Damage();
        }
    }

    private void Damage()
    {
        health--;
        healthVisualizer.UpdateHealth(health);
        if (health <= 0)
            HandleDeath();
        invincible = true;
        currentInvincibilityDurationInBeats = invincibilityDurationInBeats;
        //StartCoroutine(InvicibilityCoroutine(invincibilityDurationInBeats));

        if(!MyAudioManager.FindSound(takeDamageSoundName).source.isPlaying)
            MyAudioManager.PlaySound(takeDamageSoundName);
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
