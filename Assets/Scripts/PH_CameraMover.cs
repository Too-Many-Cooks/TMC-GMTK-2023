using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PH_CameraMover : MonoBehaviour
{
    private Camera cam;

    public Transform targetTransform;

    float shockValue = 0f;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    public void ApplyShockValue()
    {
        shockValue = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        cam.transform.LookAt(targetTransform.position);
        Quaternion q = Quaternion.LookRotation(transform.up, -cam.transform.position + targetTransform.position);
        cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, q, 0.8f * Time.deltaTime * (1f - shockValue));
        if (targetTransform.position.x - transform.position.x > 0.1f)
        {
            Vector3 newPos = transform.position;
            newPos.x = targetTransform.position.x;
            shockValue -= Time.deltaTime;
            cam.transform.position = Vector3.Lerp(transform.position, newPos, 0.8f * Time.deltaTime * (1f-shockValue));
        }
    }
}
