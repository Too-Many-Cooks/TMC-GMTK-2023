using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsBasedEnvironmentMovement : BaseEnvironmentMovement
{
    #region Self-Initiated Properties

    Rigidbody _myRigidBody;
    Rigidbody MyRigidBody
    {
        get
        {
            if(_myRigidBody == null)
                _myRigidBody = GetComponent<Rigidbody>();

            return _myRigidBody;
        }
    }

    #endregion


    const float environmentToRigidBodyMultiplier = 2;

    protected override void MoveEnvironment(Vector2 movementSpeed)
    {
        MyRigidBody.AddForce(-movementSpeed * environmentToRigidBodyMultiplier, ForceMode.VelocityChange);
    }
}
