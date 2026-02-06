using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(PlayerAnimation))]
[RequireComponent(typeof(PlayerLook))]
public class PlayerController : MonoBehaviour
{
    public PlayerMotor motor { get; private set; }
    public PlayerAnimation playerAnim { get; private set; }
    public PlayerLook look { get; private set; }

    InputSystem_Actions input;

    public Vector2 MoveInput { get; private set; }
    public bool HasMoveInput { get; private set; }

    public bool IsRunHeld { get; private set; }
    public bool JumpPressedThisFrame { get; private set; }
    public bool AttackPressedThisFrame { get; private set; }

    public Vector2 LookInput { get; private set; }

    public Vector3 CameraRelativeMoveDir { get; private set; }

    Transform camTr;
    float nextCamRetryTime;

    const float EPS = 0.0001f;

    void Awake()
    {
        motor = GetComponent<PlayerMotor>();
        playerAnim = GetComponent<PlayerAnimation>();
        look = GetComponent<PlayerLook>();

        input = new InputSystem_Actions();
        TryCacheMainCamera();
    }

    void OnEnable()  => input.Enable();
    void OnDisable() => input.Disable();

    void Update()
    {
        MoveInput = input.Player.Move.ReadValue<Vector2>();
        HasMoveInput = MoveInput.sqrMagnitude > 0.01f;

        // Shift 달리기
        IsRunHeld = input.Player.Run.IsPressed();

        // ✅ 뒤로 이동이면 달리기 막기
        if (MoveInput.y < -0.01f)
            IsRunHeld = false;

        JumpPressedThisFrame = input.Player.Jump.WasPressedThisFrame();
        AttackPressedThisFrame = input.Player.Attack.WasPressedThisFrame();

        LookInput = input.Player.Look.ReadValue<Vector2>();
        look.SetLookInput(LookInput);

        if (!camTr && Time.time >= nextCamRetryTime)
        {
            nextCamRetryTime = Time.time + 0.5f;
            TryCacheMainCamera();
        }

        CameraRelativeMoveDir = ComputeCameraRelativeMoveDir();
    }


    void TryCacheMainCamera()
    {
        var cam = Camera.main;
        camTr = cam ? cam.transform : null;
    }

    Vector3 ComputeCameraRelativeMoveDir()
    {
        if (!HasMoveInput || !camTr) return Vector3.zero;

        Vector3 f = camTr.forward; f.y = 0f;
        Vector3 r = camTr.right;   r.y = 0f;

        float fs = f.sqrMagnitude;
        if (fs < EPS) f = Vector3.forward;
        else f *= 1f / Mathf.Sqrt(fs);

        float rs = r.sqrMagnitude;
        if (rs < EPS) r = Vector3.right;
        else r *= 1f / Mathf.Sqrt(rs);

        Vector3 dir = f * MoveInput.y + r * MoveInput.x;

        float ds = dir.sqrMagnitude;
        if (ds > 1f) dir *= 1f / Mathf.Sqrt(ds);

        return dir;
    }

    public void TickAnimator()
    {
        playerAnim.SetMoveXY(MoveInput);
        playerAnim.SetSpeed01(motor.GetSpeed01());
        playerAnim.SetAir(motor.IsGrounded, motor.VerticalVelocity);
    }

}