using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Vector2Ex;

[DefaultExecutionOrder(2)]
[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float sprintSpeed = 12f;
    public float MoveSpeed { get { return isSprinting ? sprintSpeed : runSpeed; } }
    [SerializeField] private float jumpHeight = 5.5f;
    [SerializeField] private float jumpDuration = 0.625f;
    [SerializeField] private float coyoteTime = 0.05f;


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

    const string jumpSoundName = "HeroJump";
    const string swordSwingSoundName = "SwordSwing";

    #endregion


    private float DesiredGravity { get { return -8 * jumpHeight / (jumpDuration * jumpDuration); } }
    private float JumpImpulse { get { return 4 * jumpHeight / jumpDuration; } }

    private new Rigidbody2D rigidbody2D;

    private Vector2 previousPosition = Vector2.zero;
    private float desiredMove = 0f;
    private float desiredShift = 0f;
    private float currentShift = 0f;
    private float desiredShiftTime = 0f;
    private bool isSprinting = false;
    private float coyoteTimer = 0f;
    private bool tryJump = false;

    private bool isGrounded = false;

    public UnityEvent<bool> OnGrounded;

    public UnityEvent<float> OnRunning;

    void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        previousPosition = rigidbody2D.position;
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

        if (Mathf.Abs(rigidbody2D.position.y - previousPosition.y) < 0.001f) {
             rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0f);
        }

        //Figure out if we're grounded
        isGrounded = false;

        if(rigidbody2D.velocity.y <= 0f)
        {
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
        }

        OnGrounded.Invoke(isGrounded);

        if (isGrounded)
        {
            coyoteTimer = coyoteTime;
        }

        //Handle Movement
        Vector2 desiredPosition = rigidbody2D.position;

        //Handle gravity
        rigidbody2D.gravityScale = DesiredGravity / Physics2D.gravity.y;
        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, isGrounded ? 0f : rigidbody2D.velocity.y + DesiredGravity * Time.fixedDeltaTime);

        //Handle jump
        if (tryJump)
        {
            tryJump = false;
            if (isGrounded || coyoteTimer > 0f)
            {
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, JumpImpulse);
                coyoteTimer = 0f;
                isGrounded = false;

                MyAudioManager.PlaySound(jumpSoundName);
            }
        }

        desiredPosition.y += rigidbody2D.velocity.y * Time.fixedDeltaTime;

        if (desiredShift == 0f)
        {
            var xVelocity = desiredMove * MoveSpeed;

            desiredPosition.x += xVelocity * Time.fixedDeltaTime;

            OnRunning.Invoke(xVelocity);
        }
        else
        {
            var xVelocity = rigidbody2D.velocity.x;
            var oldShift = currentShift;
            currentShift = Mathf.SmoothDamp(oldShift, desiredShift, ref xVelocity, desiredShiftTime, Mathf.Infinity, Time.fixedDeltaTime);
            desiredShiftTime -= Time.fixedDeltaTime;
            desiredPosition.x += currentShift - oldShift;

            OnRunning.Invoke(xVelocity);

            if (currentShift == desiredShift)
            {
                desiredShift = 0f;
                currentShift = 0f;
                desiredShiftTime = 0f;

                OnRunning.Invoke(0);
            }
        }

        previousPosition = rigidbody2D.position;
        rigidbody2D.MovePosition(desiredPosition);
    }

    public void OnMove(float direction)
    {
        desiredMove = direction;
        desiredShift = 0f;
        currentShift = 0f;
        desiredShiftTime = 0f;
    }

    public void OnMove(InputValue value)
    {
        var move = value.Get<float>();
        OnMove(Mathf.Clamp(move, -1f, 1f));
        if(Mathf.RoundToInt(Mathf.Abs(move)) > 1)
        {
            OnSprint();
        }
    }

    public void OnMoveOver(float displacement)
    {
        desiredMove = 0f;
        currentShift = 0f;
        desiredShift = displacement;
        desiredShiftTime = Mathf.Abs(displacement) / MoveSpeed;
    }

    public void OnMoveOver(InputValue value)
    {
        OnMoveOver(value.Get<float>());
    }

    public void OnSprint(bool sprint)
    {
        isSprinting = sprint;
    }

    public void OnSprint()
    {
        OnSprint(true);
    }

    public void OnSprint(InputValue value)
    {
        OnSprint(value.Get<float>() > 0f);
    }

    public void OnJump()
    {
        tryJump = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.green : coyoteTimer > 0f ? Color.yellow : Color.red;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}