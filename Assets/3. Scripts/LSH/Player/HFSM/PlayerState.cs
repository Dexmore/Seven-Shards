public abstract class PlayerState
{
    protected PlayerStateMachine fsm;
    protected PlayerController controller;

    protected PlayerState(PlayerStateMachine fsm, PlayerController controller)
    {
        this.fsm = fsm;
        this.controller = controller;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}
