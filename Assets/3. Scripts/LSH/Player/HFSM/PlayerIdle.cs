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
        controller.playerAnim.SetSpeed01(0f);
        if (controller.HasMoveInput)
        {
            fsm.ChangeState(new PlayerWalk(fsm, controller));
        }
    }

    public void Exit() { }
}
