using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerMotor motor { get; private set; }
    public PlayerAnimation playerAnim { get; private set; }
    private InputSystem_Actions input; // 자동 생성 클래스

    public Vector2 MoveInput { get; private set; }
    public bool HasMoveInput => MoveInput.sqrMagnitude > 0.01f;
    public bool IsRunPressed => input.Player.Run.IsPressed();

    void Awake()
    {
        motor = GetComponent<PlayerMotor>();
        playerAnim = GetComponent<PlayerAnimation>();
        input = new InputSystem_Actions();
    }

    void OnEnable()
    {
        input.Enable();
    }

    void OnDisable()
    {
        input.Disable();
    }

    void Update()
    {
        MoveInput = input.Player.Move.ReadValue<Vector2>();
    }
}
