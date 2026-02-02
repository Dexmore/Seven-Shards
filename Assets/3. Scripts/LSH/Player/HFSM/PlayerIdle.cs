using UnityEngine;

public class PlayerIdle : PlayerState
{
    private PlayerStateMachine fsm;
    private PlayerController controller;

    public PlayerIdle(PlayerStateMachine fsm, PlayerController controller)
    {
        this.fsm = fsm;
        this.controller = controller;
    }

    public void Enter()
    {
        controller.motor.SetSpeed(controller.motor.walkSpeed);
        controller.playerAnim.SetSpeed01(0f);
    }

    public void Update()
    {
        if (!controller.HasMoveInput)
        {
            controller.motor.StopMove();
            controller.playerAnim.SetSpeed01(controller.motor.GetSpeed01());
            return;
        }

        if (controller.IsRunPressed)
            fsm.ChangeState(new PlayerRun(fsm, controller));
        else
            fsm.ChangeState(new PlayerWalk(fsm, controller));
    }




    public void Exit() { }
}
