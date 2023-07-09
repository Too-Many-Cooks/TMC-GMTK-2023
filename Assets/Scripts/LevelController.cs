using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class LevelController : MonoBehaviour
{
    [SerializeField] private float shiftDistance = 3f;
    [SerializeField] private float shiftDuration = 0.15f;
    [SerializeField] private float shiftCooldown = 0.5f;
    [SerializeField] private float shiftQueueTime = 0.15f;
    [SerializeField] private Vector2Int topRightClamp = new Vector2Int(5, 5);
    [SerializeField] private Vector2Int bottomLeftClamp = new Vector2Int(-5, -5);

    private float ShiftSpeed {  get { return shiftDistance / (TrueShiftDuration); } }
    private float TrueShiftDuration { get { return ShiftTicks * Time.fixedDeltaTime; } }
    private int ShiftTicks { get { return Mathf.FloorToInt(shiftDuration / Time.fixedDeltaTime); } }
    private float TrueShiftCooldown { get { return CooldownTicks * Time.fixedDeltaTime; } }
    private int CooldownTicks { get { return Mathf.FloorToInt(shiftCooldown / Time.fixedDeltaTime); } }

    private float shiftTimer = 0f;
    private float shiftCooldownTimer = 0f;
    private Vector2 origin;
    private Vector2Int queuedShift = Vector2Int.zero;
    private Vector2Int currentShift = Vector2Int.zero;
    private Vector2 shiftPosition = Vector2.zero;

    private new Rigidbody2D rigidbody2D;

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

    const string shiftEnvironmentSoundName = "MoveEnvironment";

    #endregion


    void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        origin = rigidbody2D.position;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if(shiftCooldownTimer > 0f)
        {
            shiftCooldownTimer -= Time.fixedDeltaTime;
        }
        if (shiftCooldownTimer < 0f)
        {
            shiftCooldownTimer = 0f;
        }

        if(shiftTimer > 0f)
        {
            shiftTimer -= Time.deltaTime;
        }
        if (shiftTimer < 0f)
        {
            shiftTimer = 0f;
        }

        //Handle current shift
        if (shiftTimer <= 0f && shiftCooldownTimer <= 0f && queuedShift != Vector2.zero)
        {
            currentShift += queuedShift;
            currentShift.Clamp(bottomLeftClamp, topRightClamp);
            if(currentShift == Vector2.zero)
            {
                MyAudioManager.PlaySound(shiftEnvironmentSoundName);
            }
            shiftCooldownTimer = TrueShiftCooldown;
            shiftTimer = TrueShiftDuration;
            shiftPosition = origin + (Vector2)currentShift * shiftDistance;
            queuedShift = Vector2Int.zero;
        }

        if (shiftTimer > 0f)
        {
            var velocity = rigidbody2D.velocity;
            var frameShift = Vector2.SmoothDamp(rigidbody2D.position, shiftPosition, ref velocity, shiftTimer, Mathf.Infinity, Time.fixedDeltaTime);
            rigidbody2D.velocity = velocity;
        } else
        {
            rigidbody2D.velocity = Vector2.zero;
            rigidbody2D.MovePosition(shiftPosition);
        }

        if(shiftTimer < 0f)
        {
            currentShift = Vector2Int.zero;
        }
    }

    public void OnShiftUp()
    {
        if(shiftCooldownTimer < shiftQueueTime)
        {
            queuedShift = Vector2Int.up;
            MyAudioManager.PlaySound(shiftEnvironmentSoundName);
        }
    }

    public void OnShiftDown()
    {
        if (shiftCooldownTimer < shiftQueueTime)
        {
            queuedShift = Vector2Int.down;
            MyAudioManager.PlaySound(shiftEnvironmentSoundName);
        }
    }

    public void OnShiftLeft()
    {
        if (shiftCooldownTimer < shiftQueueTime)
        {
            queuedShift = Vector2Int.left;
            MyAudioManager.PlaySound(shiftEnvironmentSoundName);
        }
    }

    public void OnShiftRight()
    {
        if (shiftCooldownTimer < shiftQueueTime)
        {
            queuedShift = Vector2Int.right;
            MyAudioManager.PlaySound(shiftEnvironmentSoundName);
        }
    }
}
