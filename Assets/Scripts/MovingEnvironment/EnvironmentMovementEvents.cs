using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentMovementEvents : MonoBehaviour
{
    [SerializeField][Range(0, 20)] float environmentSpeed = 15f;

    
    public delegate void OnEnvironmentMove(Vector2 movementAmmount);
    public static event OnEnvironmentMove onEnvironmentMove;

    // Update is called once per frame
    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        transform.localPosition -= new Vector3(input.x, input.y, 0) * environmentSpeed * Time.deltaTime;

        if (onEnvironmentMove == null)
            return;

        onEnvironmentMove(input * environmentSpeed * Time.deltaTime);
    }
}
