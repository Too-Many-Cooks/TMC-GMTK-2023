using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnimationController : MonoBehaviour
{
    public const string GroundedParameterName = "IsGrounded";
    public const string RunningParameterName = "IsRunning";
    public const string XVelocityParameterName = "VelocityX";
    public const string YVelocityParameterName = "VelocityY";


    [SerializeField] public Animator animator;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void OnRun(float xVelocity)
    {
        animator.SetBool(RunningParameterName, !Mathf.Approximately(xVelocity, 0f));
        animator.SetFloat(XVelocityParameterName, xVelocity);
    }

    public void OnGrounded(bool isGrounded)
    {
        animator.SetBool(GroundedParameterName, isGrounded);
    }
}
