public sealed class PlayerRun : PlayerState
{
    public PlayerRun(PlayerStateMachine fsm, PlayerController c) : base(fsm, c) { }

    public override void Enter()
    {
        controller.motor.SetSpeed(controller.motor.runSpeed);
    }

    public override void Update()
    {
        controller.motor.SetSpeed(controller.motor.runSpeed);

        if (controller.JumpPressedThisFrame && controller.motor.IsGrounded)
        {
            fsm.EnterAir();
            return;
        }

        if (!controller.HasMoveInput)
        {
            controller.motor.StopMove();
            fsm.ChangeLocomotion(PlayerStateMachine.Locomotion.Idle);
            return;
        }

        controller.motor.SetMoveInput(controller.GetCameraRelativeMoveDir());

        if (!controller.IsRunHeld)
            fsm.ChangeLocomotion(PlayerStateMachine.Locomotion.Walk);
    }

    public override void Exit() { }
}
