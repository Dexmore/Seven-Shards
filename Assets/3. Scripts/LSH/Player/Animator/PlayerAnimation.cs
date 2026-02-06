using UnityEngine;

[RequireComponent(typeof(Animator))]
public sealed class PlayerAnimation : MonoBehaviour
{
    Animator anim;

    static readonly int SpeedHash      = Animator.StringToHash("Speed");
    static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    static readonly int YVelocityHash  = Animator.StringToHash("YVelocity");

    [Header("Smoothing")]
    [SerializeField] float speedSmoothTime = 0.12f;
    [SerializeField] float yVelSmoothTime  = 0.08f;

    [Header("Thresholds (skip Set when tiny change)")]
    [SerializeField] float speedEpsilon = 0.001f;
    [SerializeField] float yVelEpsilon  = 0.01f;

    float speedSmoothed;
    float speedVelRef;
    float yVelSmoothed;
    float yVelVelRef;

    float lastSpeedSet = float.NaN;
    float lastYVelSet  = float.NaN;
    bool  lastGroundedSet;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void SetSpeed01(float targetSpeed01)
    {
        speedSmoothed = Mathf.SmoothDamp(speedSmoothed, targetSpeed01, ref speedVelRef, speedSmoothTime, Mathf.Infinity, Time.deltaTime);

        if (!float.IsNaN(lastSpeedSet) && Mathf.Abs(speedSmoothed - lastSpeedSet) < speedEpsilon)
            return;

        lastSpeedSet = speedSmoothed;
        anim.SetFloat(SpeedHash, speedSmoothed);
    }

    public void SetAir(bool grounded, float targetYVel)
    {
        if (grounded != lastGroundedSet)
        {
            lastGroundedSet = grounded;
            anim.SetBool(IsGroundedHash, grounded);
        }

        yVelSmoothed = Mathf.SmoothDamp(yVelSmoothed, targetYVel, ref yVelVelRef, yVelSmoothTime, Mathf.Infinity, Time.deltaTime);

        if (!float.IsNaN(lastYVelSet) && Mathf.Abs(yVelSmoothed - lastYVelSet) < yVelEpsilon)
            return;

        lastYVelSet = yVelSmoothed;
        anim.SetFloat(YVelocityHash, yVelSmoothed);
    }

    static readonly int MoveXHash = Animator.StringToHash("MoveX");
    static readonly int MoveYHash = Animator.StringToHash("MoveY");

    [SerializeField] float moveSmoothTime = 0.10f;
    [SerializeField] float moveEpsilon = 0.001f;

    float moveXSmoothed, moveYSmoothed;
    float moveXVelRef, moveYVelRef;
    float lastMoveXSet = float.NaN, lastMoveYSet = float.NaN;

    public void SetMoveXY(Vector2 moveInput)
    {
        moveXSmoothed = Mathf.SmoothDamp(moveXSmoothed, moveInput.x, ref moveXVelRef, moveSmoothTime, Mathf.Infinity, Time.deltaTime);
        moveYSmoothed = Mathf.SmoothDamp(moveYSmoothed, moveInput.y, ref moveYVelRef, moveSmoothTime, Mathf.Infinity, Time.deltaTime);

        if (float.IsNaN(lastMoveXSet) || Mathf.Abs(moveXSmoothed - lastMoveXSet) >= moveEpsilon)
        {
            lastMoveXSet = moveXSmoothed;
            anim.SetFloat(MoveXHash, moveXSmoothed);
        }

        if (float.IsNaN(lastMoveYSet) || Mathf.Abs(moveYSmoothed - lastMoveYSet) >= moveEpsilon)
        {
            lastMoveYSet = moveYSmoothed;
            anim.SetFloat(MoveYHash, moveYSmoothed);
        }
    }
}