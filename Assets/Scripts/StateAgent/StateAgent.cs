using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAgent : Agent
{
    public AgentPath path;
    public StateMachine stateMachine = new StateMachine();
    

    void Start()
    {
        stateMachine.AddState(new IdleState(this, "idle"));
        stateMachine.AddState(new PatrolState(this, "patrol"));
        stateMachine.SetState(stateMachine.StateFromName("idle"));
    }

    void Update()
    {
        stateMachine.Update();

        if (movement.velocity.magnitude > 0.5f)
        {
            animator.SetBool("IsWalking", true);
        }
        else
        {
            if (animator.GetBool("IsWalking")) animator.SetBool("IsWalking", false);
        }
    }
    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), stateMachine.GetStateName());
    }
}

