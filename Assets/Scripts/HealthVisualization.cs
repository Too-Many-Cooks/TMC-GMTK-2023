using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthVisualization : MonoBehaviour
{
    /*[SerializeField]
    Sprite fullHeart;
    [SerializeField]
    Sprite emptyHeart;*/

    [SerializeField]
    float fullAlpha = 1f;
    [SerializeField]
    float emptyAlpha = 0.25f;

    int currentHealth = 3;

    [SerializeField]
    float bounceSize = 1.2f;

    [SerializeField]
    List<Image> hearts;

    private void Start()
    {
        BeatManager.Instance.beatHitEvent.AddListener(OnBeatHit);
    }

    private void OnBeatHit(BeatManager.BeatEventData arg0)
    {
        if(arg0.beatNumberInTimeSig % 2 == 0)
        {
            StartCoroutine(HeartBeatCoroutine(arg0.estimatedTimeTillNextBeat / 2f));
        }
    }

    private IEnumerator HeartBeatCoroutine(float duration)
    {
        float currentDuration = 0f;
        while(currentDuration < duration / 2f)
        {
            SetActiveHeartScale(Mathf.Lerp(1f, bounceSize, currentDuration / (duration / 2f)));
            yield return null;
            currentDuration += Time.deltaTime;
        }
        while (currentDuration < duration)
        {
            SetActiveHeartScale(Mathf.Lerp(bounceSize, 1f, (currentDuration - duration/2f) / (duration / 2f)));
            yield return null;
            currentDuration += Time.deltaTime;
        }
        SetActiveHeartScale(1f);
    }

    private void SetActiveHeartScale(float scale)
    {
        for (int i = 0; i < currentHealth; i++)
        {
            hearts[i].transform.localScale = Vector3.one * scale;
        }
    }

    public void UpdateHealth(int newHealth)
    {
        currentHealth = newHealth;
        for (int i = 0; i < hearts.Count; i++)
        {
            if(i < newHealth)
            {
                Color newColor = hearts[i].color;
                newColor.a = fullAlpha;
                hearts[i].color = newColor;
            }
            else
            {
                Color newColor = hearts[i].color;
                newColor.a = emptyAlpha;
                hearts[i].color = newColor;
            }
        }
    }
}
