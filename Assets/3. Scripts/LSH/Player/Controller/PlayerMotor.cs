using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    [Header("Move")]
    public float walkSpeed = 4f;
    public float accel = 25f;
    public float decel = 35f;

    private Rigidbody rb;
    private Vector3 targetVel;
    private float speed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        speed = walkSpeed;
    }

    public void SetSpeed(float newSpeed) => speed = newSpeed;

    public void SetMoveInput(Vector2 input)
    {
        Vector3 dir = new Vector3(input.x, 0f, input.y);
        dir = Vector3.ClampMagnitude(dir, 1f);

        targetVel = dir * speed;

        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 12f * Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        Vector3 v = rb.linearVelocity;
        Vector3 curH = new Vector3(v.x, 0f, v.z);

        float a = (targetVel.sqrMagnitude > 0.01f) ? accel : decel;
        Vector3 newH = Vector3.MoveTowards(curH, targetVel, a * Time.fixedDeltaTime);

        rb.linearVelocity = new Vector3(newH.x, v.y, newH.z);
    }
}
