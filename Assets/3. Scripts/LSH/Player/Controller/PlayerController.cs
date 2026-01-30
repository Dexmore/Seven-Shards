using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerMotor motor { get; private set; }
    public PlayerAnimation playerAnim { get; private set; }

    public Vector2 MoveInput { get; private set; }
    public bool HasMoveInput => MoveInput.sqrMagnitude > 0.01f;

    void Awake()
    {
        motor = GetComponent<PlayerMotor>();
        playerAnim = GetComponent<PlayerAnimation>();
    }

    void Update()
    {
        Vector2 move = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed) move.x -= 1f;
            if (Keyboard.current.dKey.isPressed) move.x += 1f;
            if (Keyboard.current.sKey.isPressed) move.y -= 1f;
            if (Keyboard.current.wKey.isPressed) move.y += 1f;
        }
        MoveInput = Vector2.ClampMagnitude(move, 1f);
    }
}
