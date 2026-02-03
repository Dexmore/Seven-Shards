using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public sealed class PlayerMotor : MonoBehaviour
{
    [Header("Speed")]
    public float walkSpeed = 4f;
    public float runSpeed  = 7f;

    [Header("Accel / Decel")]
    public float accel = 25f;
    public float decel = 40f;

    [Header("Turn Rate (deg/sec)")]
    public float walkTurnRateDeg = 360f;
    public float runTurnRateDeg  = 900f;

    private Rigidbody rb;

    private float speed;
    private Vector3 targetDir;
    private float targetSpeed;
    private float invRunSpeed;

    [Header("Ground / Jump")]
    public Transform groundCheck;
    public LayerMask groundMask = ~0;
    public float groundCheckRadius = 0.25f;

    public float jumpHeight = 1.6f;

    public bool IsGrounded { get; private set; }
    private float _groundIgnoreUntil;

    public float VerticalVelocity => rb.linearVelocity.y;

    private const float EPS = 0.0001f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        speed = walkSpeed;
        invRunSpeed = runSpeed > EPS ? 1f / runSpeed : 0f;
    }

    public void SetSpeed(float newSpeed) => speed = Mathf.Max(0f, newSpeed);

    public void SetMoveInput(Vector3 worldDir)
    {
        worldDir.y = 0f;
        float sqr = worldDir.sqrMagnitude;

        if (sqr > EPS)
        {
            if (sqr > 1f) worldDir.Normalize();
            targetDir = worldDir;
            targetSpeed = speed;
        }
        else
        {
            targetDir = Vector3.zero;
            targetSpeed = 0f;
        }
    }

    public void StopMove()
    {
        targetDir = Vector3.zero;
        targetSpeed = 0f;
    }

    public float GetSpeed01()
    {
        Vector3 v = rb.linearVelocity;
        float hsqr = v.x * v.x + v.z * v.z;
        float h = (hsqr > EPS) ? Mathf.Sqrt(hsqr) : 0f;
        return Mathf.Clamp01(h * invRunSpeed);
    }

    private void UpdateGrounded()
    {
        if (Time.time < _groundIgnoreUntil)
        {
            IsGrounded = false;
            return;
        }

        Vector3 origin = groundCheck ? groundCheck.position : (transform.position + Vector3.up * 0.1f);

        IsGrounded = Physics.CheckSphere(
            origin,
            groundCheckRadius,
            groundMask,
            QueryTriggerInteraction.Ignore
        );
    }

    public void DoJump()
    {
        float g = Mathf.Abs(Physics.gravity.y);
        float jumpVel = Mathf.Sqrt(2f * g * Mathf.Max(0.01f, jumpHeight));

        Vector3 v = rb.linearVelocity;
        v.y = jumpVel;
        rb.linearVelocity = v;

        IsGrounded = false;
        _groundIgnoreUntil = Time.time + 0.10f;
    }

    private void FixedUpdate()
    {
        UpdateGrounded();

        if (targetDir == Vector3.zero)
            targetSpeed = 0f;

        float dt = Time.fixedDeltaTime;

        Vector3 v = rb.linearVelocity;

        Vector3 horizontalVel = new Vector3(v.x, 0f, v.z);
        float currentSpeed = horizontalVel.magnitude;

        float a = targetSpeed > 0.01f ? accel : decel;
        float newSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, a * dt);

        Vector3 moveDir = targetDir;
        if (moveDir == Vector3.zero)
        {
            if (currentSpeed > 0.001f)
                moveDir = horizontalVel / currentSpeed;
            else
                moveDir = Vector3.zero;
        }

        rb.linearVelocity = new Vector3(moveDir.x * newSpeed, v.y, moveDir.z * newSpeed);
    }

}