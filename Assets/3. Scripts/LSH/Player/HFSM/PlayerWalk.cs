public sealed class PlayerWalk : PlayerState
{
    public PlayerWalk(PlayerStateMachine fsm, PlayerController c) : base(fsm, c) { }

    public override void Enter()
    {
        controller.motor.SetSpeed(controller.motor.walkSpeed);
    }

    public override void Update()
    {
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

        float speed01 = controller.motor.GetSpeed01();
        controller.playerAnim.SetSpeed01(speed01);

        if (controller.IsRunHeld)
            fsm.ChangeLocomotion(PlayerStateMachine.Locomotion.Run);
    }

    public override void Exit() { }
}
