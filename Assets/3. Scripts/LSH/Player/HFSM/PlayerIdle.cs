using UnityEngine;
public sealed class PlayerIdle : PlayerState
{
    public PlayerIdle(PlayerStateMachine fsm, PlayerController c) : base(fsm, c) { }

    public override void Enter()
    {
        // Idle 들어올 때만 세팅 (매 프레임 X)
        pc.motor.SetSpeed(pc.motor.walkSpeed);
        pc.motor.SetMoveInput(Vector3.zero);
    }

    public override void Tick()
    {
        if (pc.JumpPressedThisFrame && pc.motor.IsGrounded)
        {
            fsm.EnterAir();
            return;
        }

        if (!pc.HasMoveInput)
            return;

        fsm.ChangeLocomotion(pc.IsRunHeld? PlayerStateMachine.Locomotion.Run: PlayerStateMachine.Locomotion.Walk);
    }

    public override void Exit() { }
}