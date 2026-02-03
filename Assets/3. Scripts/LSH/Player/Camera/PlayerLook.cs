using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public sealed class PlayerLook : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Transform cameraPivot;

    [Header("Sensitivity")]
    [SerializeField] float mouseSensitivity = 0.12f;

    [Header("Pitch Clamp")]
    [SerializeField] float minPitch = -35f;
    [SerializeField] float maxPitch = 70f;

    Rigidbody rb;

    float yaw;
    float pitch;

    Vector2 lookDeltaAccum;
    float yawToApply;

    public float Yaw   => yaw;
    public float Pitch => pitch;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        yaw = transform.eulerAngles.y;

        if (cameraPivot)
        {
            float p = cameraPivot.localEulerAngles.x;
            if (p > 180f) p -= 360f;
            pitch = Mathf.Clamp(p, minPitch, maxPitch);
        }
    }

    public void SetLookInput(Vector2 delta)
    {
        lookDeltaAccum += delta;
    }

    void Update()
    {
        if (lookDeltaAccum.sqrMagnitude < 0.0001f)
            return;

        float sens = mouseSensitivity;

        yaw   += lookDeltaAccum.x * sens;
        pitch -= lookDeltaAccum.y * sens;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        yawToApply = yaw;
        lookDeltaAccum = Vector2.zero;
    }

    void LateUpdate()
    {
        if (cameraPivot)
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void FixedUpdate()
    {
        rb.MoveRotation(Quaternion.Euler(0f, yawToApply, 0f));
    }
}