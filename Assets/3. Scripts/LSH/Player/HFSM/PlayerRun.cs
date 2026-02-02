public class PlayerRun : PlayerState
{
    private PlayerStateMachine fsm;
    private PlayerController controller;

    public PlayerRun(PlayerStateMachine fsm, PlayerController controller)
    {
        this.fsm = fsm;
        this.controller = controller;
    }

    public void Enter()
    {
        controller.motor.SetSpeed(controller.motor.runSpeed);
    }

    public void Update()
    {
        if (!controller.HasMoveInput)
        {
            controller.motor.StopMove();
            fsm.ChangeState(new PlayerIdle(fsm, controller));
            return;
        }

        controller.motor.SetMoveInput(controller.MoveInput);

        controller.playerAnim.SetSpeed01(controller.motor.GetSpeed01());

        if (!controller.IsRunPressed)
            fsm.ChangeState(new PlayerWalk(fsm, controller));
    }


    public void Exit() { }
}
