using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BeatManager;

public class Debug_BeatColorSwitcher : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private Material mat;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        mat = Instantiate(meshRenderer.material);
        meshRenderer.material = mat;
    }

    public void OnBeatHit(BeatEventData beatEventData)
    {
        switch(beatEventData.beatNumberInTimeSig)
        {
            case 0:
                mat.color = Color.red;
                StartCoroutine(BounceCoroutine(0.1f));
                break;
            case 1:
                mat.color = Color.blue;
                break;
            case 2:
                mat.color = Color.green;
                break;
            case 3:
                mat.color = Color.white;
                break;

        }
    }

    private IEnumerator BounceCoroutine(float duration)
    {
        float elapsed = 0f;
        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;
            Vector3 scale = Vector3.one * Mathf.Lerp(1f, 1.5f, elapsed / duration);
            transform.localScale = scale;
            yield return null;
        }
        while (elapsed < 2f * duration)
        {
            elapsed += Time.deltaTime;
            Vector3 scale = Vector3.one * Mathf.Lerp(1.5f, 1f, (elapsed - duration) / duration);
            transform.localScale = scale;
            yield return null;
        }
    }
}
