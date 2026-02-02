public sealed class PlayerIdle : PlayerState
{
    public PlayerIdle(PlayerStateMachine fsm, PlayerController c) : base(fsm, c) { }

    public override void Enter()
    {
        controller.motor.SetSpeed(controller.motor.walkSpeed);
        controller.motor.StopMove();
    }

    public override void Update()
    {
        if (controller.JumpPressedThisFrame && controller.motor.IsGrounded)
        {
            fsm.EnterAir();
            return;
        }

        // 스무스 감속 애니
        controller.playerAnim.SetSpeed01(controller.motor.GetSpeed01());

        if (!controller.HasMoveInput) return;

        fsm.ChangeLocomotion(controller.IsRunHeld
            ? PlayerStateMachine.Locomotion.Run
            : PlayerStateMachine.Locomotion.Walk);
    }

    public override void Exit() { }
}
