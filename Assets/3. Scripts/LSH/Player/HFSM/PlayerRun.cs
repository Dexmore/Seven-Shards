using UnityEngine;

public sealed class PlayerRun : PlayerState
{
    private Vector3 _lastMoveDir;

    public PlayerRun(PlayerStateMachine fsm, PlayerController pc) : base(fsm, pc) { }

    public override void Enter()
    {
        pc.motor.SetSpeed(pc.motor.runSpeed);
        _lastMoveDir = Vector3.zero;
    }

    public override void Tick()
    {
        if (pc.JumpPressedThisFrame && pc.motor.IsGrounded)
        {
            SetMoveDir(Vector3.zero);
            fsm.EnterAir();
            return;
        }

        if (!pc.HasMoveInput)
        {
            SetMoveDir(Vector3.zero);
            fsm.ChangeLocomotion(PlayerStateMachine.Locomotion.Idle);
            return;
        }

        SetMoveDir(pc.CameraRelativeMoveDir);

        if (!pc.IsRunHeld)
            fsm.ChangeLocomotion(PlayerStateMachine.Locomotion.Walk);
    }

    private void SetMoveDir(Vector3 dir)
    {
        if ((dir - _lastMoveDir).sqrMagnitude < 0.0001f)
            return;

        _lastMoveDir = dir;
        pc.motor.SetMoveInput(dir);
    }

    public override void Exit() { }
}