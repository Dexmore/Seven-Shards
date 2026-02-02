public sealed class PlayerAir : PlayerState
{
    public PlayerAir(PlayerStateMachine fsm, PlayerController c) : base(fsm, c) { }

    public override void Enter()
    {
        controller.motor.DoJump();
    }

    public override void Update()
    {
        if (controller.HasMoveInput)
            controller.motor.SetMoveInput(controller.GetCameraRelativeMoveDir());
        else
            controller.motor.StopMove();

        float speed01 = controller.motor.GetSpeed01();
        controller.playerAnim.SetSpeed01(speed01);

        if (!controller.motor.IsGrounded) return;

        if (!controller.HasMoveInput)
        {
            fsm.ChangeLocomotion(PlayerStateMachine.Locomotion.Idle);
            return;
        }

        fsm.ChangeLocomotion(controller.IsRunHeld
            ? PlayerStateMachine.Locomotion.Run
            : PlayerStateMachine.Locomotion.Walk);
    }

    public override void Exit() { }
}