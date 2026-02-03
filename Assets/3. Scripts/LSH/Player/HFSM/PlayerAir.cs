using UnityEngine;

public sealed class PlayerAir : PlayerState
{
    private Vector3 _lastMoveDir;

    public PlayerAir(PlayerStateMachine fsm, PlayerController pc) : base(fsm, pc) { }

    public override void Enter()
    {
        pc.motor.DoJump();
        _lastMoveDir = Vector3.zero;
    }

    public override void Tick()
    {
        if (pc.HasMoveInput)
            SetMoveDir(pc.CameraRelativeMoveDir);
        else
            SetMoveDir(Vector3.zero);

        if (!pc.motor.IsGrounded)
            return;

        if (!pc.HasMoveInput)
        {
            fsm.ChangeLocomotion(PlayerStateMachine.Locomotion.Idle);
            return;
        }

        fsm.ChangeLocomotion(pc.IsRunHeld
            ? PlayerStateMachine.Locomotion.Run
            : PlayerStateMachine.Locomotion.Walk);
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