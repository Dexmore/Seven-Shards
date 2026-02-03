public abstract class PlayerState
{
    protected readonly PlayerStateMachine fsm;
    protected readonly PlayerController pc;

    protected PlayerState(PlayerStateMachine fsm, PlayerController pc)
    {
        this.fsm = fsm;
        this.pc = pc;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }

    public virtual void Tick() { }
    public virtual void FixedTick() { }
    public virtual void LateTick() { }
}
