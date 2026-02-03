using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public sealed class PlayerLook : MonoBehaviour
{
    [Header("Refs")]
    public Transform cameraPivot;
    private Rigidbody rb;

    [Header("Sensitivity")]
    public float mouseSensitivity = 0.12f;

    [Header("Pitch Clamp")]
    public float minPitch = -35f;
    public float maxPitch = 70f;

    private float yaw;
    private float pitch;

    private Vector2 lookDelta;

    public float Yaw => yaw;
    public float Pitch => pitch;

    public void SetLookInput(Vector2 delta) => lookDelta = delta;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        yaw = transform.eulerAngles.y;

        if (cameraPivot != null)
        {
            float p = cameraPivot.localEulerAngles.x;
            if (p > 180f) p -= 360f;
            pitch = Mathf.Clamp(p, minPitch, maxPitch);
        }
    }

    private void LateUpdate()
    {
        yaw   += lookDelta.x * mouseSensitivity;
        pitch -= lookDelta.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        if (cameraPivot != null)
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void FixedUpdate()
    {
        rb.MoveRotation(Quaternion.Euler(0f, yaw, 0f));
    }
}
