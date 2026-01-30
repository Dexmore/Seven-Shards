using System;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    public PlayerState currentState { get; private set; }
    PlayerController pc;

    private void Awake()
    {
        pc = GetComponent<PlayerController>();
    }

    private void Start()
    {
        ChangeState(new PlayerIdle(this, pc));
    }
    public void ChangeState(PlayerState newstate)
    {
        currentState?.Exit(); // Exit current State
        currentState = newstate; // Get new State
        currentState?.Enter(); // Enter with new State
    }

    // Update is called once per frame
    void Update()
    {
        currentState?.Update();
    }
}
