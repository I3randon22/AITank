using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class FiniteStateMachine_ThomasTheTank_FSM : MonoBehaviour
{
    /*
    private enum State
    {
        Patrol, // Tank is searching around for anything it sees
        Chasing, // Enemy Tank found, chasing it down to close enough shooting range
        AttackingTank, // Enemy Tank is close enough, begin attacking the Tank
        AttackingBase, // Attack the Enemyy Base
        Retreat, //Tank is low on specific item / Caugth attacking the Enemy Base
        LowStat // Tank is low on a specific stat, cycle through found items for the closest needed item
    }

    private State enum_currentState;

    */


    private Dictionary<Type, BaseState_ThomasTheTank_FSM> states;
    private BaseState_ThomasTheTank_FSM currentState;

    public BaseState_ThomasTheTank_FSM CurrentState
    {
        get
        {
            return currentState;
        }
        set
        {
            currentState = value;
        }
    }

    public void SetStates(Dictionary<Type, BaseState_ThomasTheTank_FSM> states)
    {
        this.states = states;
    }

    public void Update()
    {
        if (CurrentState == null)
        {
            CurrentState = states.Values.First();
        }
        else
        {
            //CurrentState.StateUpdate();
            var nextState = CurrentState.StateUpdate();

            if(nextState != null && nextState != CurrentState.GetType())
            {
                SwitchToState(nextState);
            }
        }



        /*
        switch(enum_currentState)
        {
            case State.Patrol:
                Debug.Log("Patrolling");

                break;

            case State.Chasing:
                Debug.Log("Chasing");

                break;


            case State.AttackingTank:
                Debug.Log("Attacking Enemy Tank");

                break;

            case State.AttackingBase:
                Debug.Log("Attacking Enemy Base");

                break;

            case State.Retreat:
                Debug.Log("Retreating");

                break;

            case State.LowStat:
                Debug.Log("Looking for item needed");

                break;
        }
        */
    }

    void SwitchToState(Type nextState)
    {
        CurrentState.StateExit();
        CurrentState = states[nextState];
        CurrentState.StateEnter();
    }
}
