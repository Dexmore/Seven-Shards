using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    [Header("Speed")]
    public float walkSpeed = 4f;
    public float runSpeed = 7f;

    [Header("Accel/Decel")]
    public float accel = 25f;
    public float decel = 12f;

    [Header("Turning")]
    public float turnRateDeg = 1080f;

    private Rigidbody rb;

    private float speed;
    private Vector3 targetDir = Vector3.zero;
    private float targetSpeed = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        speed = walkSpeed;
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void SetMoveInput(Vector2 input)
    {
        Vector3 dir = new Vector3(input.x, 0f, input.y);
        dir = Vector3.ClampMagnitude(dir, 1f);

        if (dir.sqrMagnitude > 0.0001f)
        {
            targetDir = dir.normalized;
            targetSpeed = dir.magnitude * speed;
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

    /// <summary>
    /// 현재 실제 수평 속도를 0~1로 정규화 (runSpeed 기준)
    /// </summary>
    public float GetSpeed01()
    {
        Vector3 v = rb.linearVelocity;
        Vector3 h = new Vector3(v.x, 0f, v.z);
        return Mathf.Clamp01(h.magnitude / runSpeed);
    }

    void FixedUpdate()
    {
        Vector3 v = rb.linearVelocity;
        Vector3 curH = new Vector3(v.x, 0f, v.z);

        float curSpeed = curH.magnitude;

        float a = (targetSpeed > 0.01f) ? accel : decel;
        float newSpeed = Mathf.MoveTowards(curSpeed, targetSpeed, a * Time.fixedDeltaTime);

        Vector3 curDir;
        if (curSpeed > 0.001f)
            curDir = curH / curSpeed;
        else
            curDir = (targetDir != Vector3.zero) ? targetDir : transform.forward;

        Vector3 newDir = curDir;
        if (targetDir != Vector3.zero)
        {
            newDir = Vector3.RotateTowards(
                curDir,
                targetDir,
                Mathf.Deg2Rad * turnRateDeg * Time.fixedDeltaTime,
                0f
            );
        }

        Vector3 newH = newDir * newSpeed;

        rb.linearVelocity = new Vector3(newH.x, v.y, newH.z);

        if (newH.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(newDir, Vector3.up);
    }
}
