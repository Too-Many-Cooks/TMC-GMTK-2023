using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class TorchMovement : BaseEnvironmentMovement
{
    [SerializeField][Range(0, 90)] float maxAmmountOfZRotation = 25;

    float zRotation = 0;

    const float environmentToTorchSpeedMultiplier = 4000;

    protected override void MoveEnvironment(Vector2 movementSpeed)
    {
        if (movementSpeed.x == 0)
            return;

        float rotationalSpeed = movementSpeed.x;

        zRotation += rotationalSpeed * environmentToTorchSpeedMultiplier * Time.deltaTime;


        if(Mathf.Abs(zRotation) > maxAmmountOfZRotation)
            zRotation = maxAmmountOfZRotation * Mathf.Sign(zRotation);

        transform.localEulerAngles = new Vector3(0, 0, zRotation);
    }
}
