using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerStateMachine : MonoBehaviour
{
    public enum Locomotion { Idle, Walk, Run }

    public PlayerState Current { get; private set; }

    private PlayerController pc;

    private PlayerIdle idle;
    private PlayerWalk walk;
    private PlayerRun run;
    private PlayerAir air;

    private void Awake()
    {
        pc = GetComponent<PlayerController>();

        idle = new PlayerIdle(this, pc);
        walk = new PlayerWalk(this, pc);
        run  = new PlayerRun(this, pc);
        air  = new PlayerAir(this, pc);
    }

    private void Start()
    {
        ChangeState(idle);
    }

    private void Update()
    {
        Current?.Update();
        pc.TickAnimator();
    }

    private void ChangeState(PlayerState next)
    {
        if (Current == next) return;
        Current?.Exit();
        Current = next;
        Current.Enter();
    }

    public void ChangeLocomotion(Locomotion next)
    {
        switch (next)
        {
            case Locomotion.Idle: ChangeState(idle); break;
            case Locomotion.Walk: ChangeState(walk); break;
            case Locomotion.Run:  ChangeState(run);  break;
        }
    }

    public void EnterAir()
    {
        ChangeState(air);
    }
}
