using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    public enum Locomotion { Idle, Walk, Run }

    public PlayerState Current { get; private set; }

    PlayerController pc;

    PlayerIdle idle;
    PlayerWalk walk;
    PlayerRun run;
    PlayerAir air;

    void Awake()
    {
        pc = GetComponent<PlayerController>();

        idle = new PlayerIdle(this, pc);
        walk = new PlayerWalk(this, pc);
        run  = new PlayerRun(this, pc);
        air  = new PlayerAir(this, pc);

        ChangeState(idle);
    }

    void Update()
    {
        Current.Tick();
    }

    void FixedUpdate()
    {
        Current.FixedTick();
    }

    void LateUpdate()
    {
        Current.LateTick();
        pc.TickAnimator();
    }

    private void ChangeState(PlayerState next)
    {
        if (ReferenceEquals(Current, next)) return;

        if (Current != null)
            Current.Exit();

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

    public void EnterAir() => ChangeState(air);
}