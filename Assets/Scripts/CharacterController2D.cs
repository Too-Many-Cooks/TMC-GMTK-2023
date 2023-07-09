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

    [SerializeField] private AttackData attack;
    private float attackTimer = 0f;
    private float attackCooldown = 0f;
    private GameObject hitbox;
    private BoxCollider2D hitboxCollider;


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
    public UnityEvent<bool> OnAttacking;

    private float xVelocity;

    void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        previousPosition = rigidbody2D.position;
        hitbox = new GameObject("Hurtbox");
        hitbox.transform.parent = this.transform;
        hitbox.transform.localPosition = Vector3.zero;
        hitboxCollider = hitbox.AddComponent<BoxCollider2D>();
        hitbox.AddComponent<SwordKilling>();
        hitboxCollider.enabled = false;
        hitboxCollider.isTrigger = true;
    }

    void FixedUpdate()
    {
        CalculateMovement();
        CalculateAttack();
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
            xVelocity = desiredMove * MoveSpeed;

            desiredPosition.x += xVelocity * Time.fixedDeltaTime;

            OnRunning.Invoke(xVelocity);
        }
        else
        {
            xVelocity = rigidbody2D.velocity.x;
            var oldShift = currentShift;
            currentShift = Mathf.SmoothDamp(oldShift, desiredShift, ref xVelocity, desiredShiftTime, Mathf.Infinity, Time.fixedDeltaTime);
            desiredShiftTime -= Time.fixedDeltaTime;
            desiredPosition.x += currentShift - oldShift;

            if (currentShift == desiredShift)
            {
                xVelocity = 0;
                desiredShift = 0f;
                currentShift = 0f;
                desiredShiftTime = 0f;
            }

            OnRunning.Invoke(xVelocity);
        }

        previousPosition = rigidbody2D.position;
        rigidbody2D.MovePosition(desiredPosition);
    }

    private void CalculateAttack()
    {
        if (attackTimer > 0f)
        {
            var time = 1 - attackTimer / attack.attackDuration;
            hitboxCollider.offset = attack.meleeAttackCollider.position * Mathf.Sign(xVelocity);
            hitboxCollider.size = attack.meleeAttackCollider.size;
            hitboxCollider.enabled = true;

            attackTimer -= Time.fixedDeltaTime;

            OnAttacking.Invoke(true);
        }
        else
        {
            hitboxCollider.enabled = false;

            OnAttacking.Invoke(false);
        }
        if (attackTimer < 0f)
        {
            attackTimer = 0f;
        }

        attackCooldown -= Time.fixedDeltaTime;
        if (attackCooldown < 0f)
        {
            attackCooldown = 0f;
        }
    }

    public void OnAttack()
    {
        if(attackCooldown <= 0f)
        {
            attackTimer = attack.attackDuration;
            attackCooldown = attack.attackCooldown;
            hitboxCollider.offset = attack.meleeAttackCollider.position * Mathf.Sign(xVelocity);
            hitboxCollider.size = attack.meleeAttackCollider.size;
            hitboxCollider.enabled = true;

            OnAttacking.Invoke(true);
        }
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