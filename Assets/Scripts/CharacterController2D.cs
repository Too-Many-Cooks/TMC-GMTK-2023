using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Vector2Ex;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float runSpeed = 12f;
    [SerializeField] private float jumpHeight = 5.5f;
    [SerializeField] private float jumpDuration = 0.625f;
    [SerializeField] private float coyoteTime = 0.05f;

    private float DesiredGravity { get { return -8 * jumpHeight / (jumpDuration * jumpDuration); } }
    private float JumpImpulse { get { return 4 * jumpHeight / jumpDuration; } }

    private new Rigidbody2D rigidbody2D;

    private float desiredMove = 0f;
    private float coyoteTimer = 0f;
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
        if (!isGrounded && coyoteTimer > 0f)
        {
            coyoteTimer -= Time.fixedDeltaTime;
            if(coyoteTimer < 0f)
            {
                coyoteTimer = 0f;
            }
        }

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

        if(isGrounded)
        {
            coyoteTimer = coyoteTime;
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
            if(isGrounded || coyoteTimer > 0f)
            {
                desiredVelocity.y = JumpImpulse;
                coyoteTimer = 0f;
                isGrounded = false;
            }
        }

        var hits = new List<RaycastHit2D>();
        var desiredStep = desiredVelocity * Time.fixedDeltaTime;
        if (rigidbody2D.Cast(desiredVelocity, hits, desiredStep.magnitude) > 0)
        {
            RaycastHit2D shortestHit = new RaycastHit2D { distance = float.MaxValue };
            foreach (var hit in hits)
            {
                if (hit.distance < shortestHit.distance)
                {
                    shortestHit = hit;
                }
            }

            var safeStep = desiredVelocity.normalized * shortestHit.distance;
            var remainingStep = desiredStep - safeStep;

            var lostStep = remainingStep.Project(shortestHit.normal);

            desiredStep -= lostStep;

            desiredVelocity = desiredStep / Time.deltaTime;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.green : coyoteTimer > 0f ? Color.yellow : Color.red;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}