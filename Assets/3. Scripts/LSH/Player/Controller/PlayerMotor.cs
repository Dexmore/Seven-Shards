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

    [Header("Ground")]
    public Transform groundCheck;
    public LayerMask groundMask = ~0;
    public float groundCheckRadius = 0.25f;
    public float groundCheckInterval = 0.05f;

    [Header("Jump")]
    public float jumpHeight = 1.6f;

    Rigidbody rb;

    Vector3 targetDir;
    float targetSpeed;
    float currentSpeed;

    float speed;
    float invRunSpeed;

    float nextGroundCheckTime;
    float groundIgnoreUntil;

    public bool IsGrounded { get; private set; }
    public float VerticalVelocity => rb.linearVelocity.y;
    public float MoveScale { get; private set; } = 1f;
    const float EPS = 0.0001f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        speed = walkSpeed;
        invRunSpeed = runSpeed > EPS ? 1f / runSpeed : 0f;
    }

    void Update()
    {
        UpdateGrounded();
    }

    public void SetSpeed(float newSpeed)
    {
        speed = Mathf.Max(0f, newSpeed);
    }

    public void SetMoveScale(float scale)
    {
        MoveScale = Mathf.Clamp01(scale);
    }
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

    void UpdateGrounded()
    {
        if (Time.time < nextGroundCheckTime)
            return;

        nextGroundCheckTime = Time.time + groundCheckInterval;

        if (Time.time < groundIgnoreUntil)
        {
            IsGrounded = false;
            return;
        }

        Vector3 origin = groundCheck? groundCheck.position: transform.position + Vector3.up * 0.1f;

        IsGrounded = Physics.CheckSphere(origin, groundCheckRadius, groundMask, QueryTriggerInteraction.Ignore);
    }

    public void DoJump()
    {
        float g = -Physics.gravity.y;
        float jumpVel = Mathf.Sqrt(2f * g * jumpHeight);

        Vector3 v = rb.linearVelocity;
        v.y = jumpVel;
        rb.linearVelocity = v;

        IsGrounded = false;
        groundIgnoreUntil = Time.time + 0.1f;
    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        Vector3 v = rb.linearVelocity;
        Vector3 horizontal = new Vector3(v.x, 0f, v.z);

        float target = targetSpeed;
        float a = target > currentSpeed ? accel : decel;

        currentSpeed = Mathf.MoveTowards(currentSpeed, target * MoveScale,a * dt);

        Vector3 dir =targetDir.sqrMagnitude > EPS? targetDir: (horizontal.sqrMagnitude > EPS ? horizontal.normalized : Vector3.zero);

        rb.linearVelocity = new Vector3(dir.x * currentSpeed, v.y, dir.z * currentSpeed);
    }

    public float GetSpeed01()
    {
        return Mathf.Clamp01(currentSpeed * invRunSpeed);
    }
}