using UnityEngine;

public class PlayerWalk : PlayerState
{
    private PlayerStateMachine fsm;
    private PlayerController controller;

    public PlayerWalk(PlayerStateMachine fsm, PlayerController controller)
    {
        this.fsm = fsm;
        this.controller = controller;
    }

    public void Enter()
    {
        controller.motor.SetSpeed(controller.motor.walkSpeed);
        // TODO: Animator Walk
    }

    public void Update()
    {
        controller.motor.SetMoveInput(controller.MoveInput);
        controller.playerAnim.SetSpeed01(controller.MoveInput.magnitude); // 0~1
        if (!controller.HasMoveInput)
        {
            fsm.ChangeState(new PlayerIdle(fsm, controller));
        }
    }

    public void Exit() { }
}
