using UnityEngine;

public sealed class PlayerController : MonoBehaviour
{
    public PlayerMotor motor { get; private set; }
    public PlayerAnimation playerAnim { get; private set; }

    private InputSystem_Actions input;

    public Vector2 MoveInput { get; private set; }
    public bool HasMoveInput => MoveInput.sqrMagnitude > 0.01f;

    public bool IsRunHeld { get; private set; }
    public bool JumpPressedThisFrame { get; private set; }

    private Transform _camTr;
    private int _camCacheFrame = -1;

    private void Awake()
    {
        motor = GetComponent<PlayerMotor>();
        playerAnim = GetComponent<PlayerAnimation>();
        input = new InputSystem_Actions();
    }

    private void OnEnable() => input.Enable();
    private void OnDisable() => input.Disable();

    private void Update()
    {
        MoveInput = input.Player.Move.ReadValue<Vector2>();
        IsRunHeld = input.Player.Run.IsPressed();
        JumpPressedThisFrame = input.Player.Jump.WasPressedThisFrame();
    }

    public void TickAnimator()
    {
        playerAnim.SetSpeed01(motor.GetSpeed01());
        playerAnim.SetAir(motor.IsGrounded, motor.VerticalVelocity);
    }

    public Vector3 GetCameraRelativeMoveDir()
    {
        if (!HasMoveInput) return Vector3.zero;

        if (_camCacheFrame != Time.frameCount)
        {
            _camCacheFrame = Time.frameCount;

            if (_camTr == null)
            {
                var cam = Camera.main;
                _camTr = cam != null ? cam.transform : null;
            }
        }

        if (_camTr == null) return Vector3.zero;

        Vector3 f = _camTr.forward; f.y = 0f;
        Vector3 r = _camTr.right;   r.y = 0f;

        if (f.sqrMagnitude > 0.0001f) f.Normalize(); else f = Vector3.forward;
        if (r.sqrMagnitude > 0.0001f) r.Normalize(); else r = Vector3.right;

        Vector3 dir = f * MoveInput.y + r * MoveInput.x;
        if (dir.sqrMagnitude > 1f) dir.Normalize();
        return dir;
    }
}