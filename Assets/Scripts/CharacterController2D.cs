using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float jumpHeight = 5.5f;
    [SerializeField] private float jumpDuration = 0.625f;

    private float DesiredGravity { get { return -8 * jumpHeight / (jumpDuration * jumpDuration); } }
    private float JumpImpulse { get { return 4 * jumpHeight / jumpDuration; } }

    private new Rigidbody2D rigidbody2D;

    private float desiredMove = 0f;
    private bool tryJump = false;

    private bool isGrounded = false;

    void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        
    }

    void FixedUpdate()
    {
        CalculateMovement();
    }

    private void CalculateMovement()
    {

        //Figure out if we're grounded
        isGrounded = false;

        var contacts = new List<ContactPoint2D>();
        rigidbody2D.GetContacts(contacts);
        foreach (var contact in contacts)
        {
            if (contact.normal.y > Mathf.Abs(contact.normal.x))
            {
                isGrounded = true;
                break;
            }
        }

        //Handle gravity
        rigidbody2D.gravityScale = DesiredGravity / Physics2D.gravity.y;

        //Calculate run
        Vector2 desiredVelocity = rigidbody2D.velocity;
        desiredVelocity.x = desiredMove * runSpeed;

        //Handle jump
        if(tryJump)
        {
            tryJump = false;
            if(isGrounded)
            {
                desiredVelocity.y = JumpImpulse;
                isGrounded = false;
            }
        }

        rigidbody2D.velocity = desiredVelocity;
    }

    void  OnMove(InputValue value)
    {
        desiredMove = value.Get<float>();
    }

    void OnJump()
    {
        tryJump = true;
    }
}