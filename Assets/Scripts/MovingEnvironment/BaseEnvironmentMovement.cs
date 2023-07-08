using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnvironmentMovement : MonoBehaviour
{
    protected abstract void MoveEnvironment(Vector2 movementSpeed);


    private void OnEnable()
    {
        EnvironmentMovementEvents.onEnvironmentMove += MoveEnvironment;
    }

    private void OnDisable()
    {
        EnvironmentMovementEvents.onEnvironmentMove -= MoveEnvironment;
    }
}
