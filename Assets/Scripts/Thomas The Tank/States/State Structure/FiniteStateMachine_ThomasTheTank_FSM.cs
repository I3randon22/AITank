using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class FiniteStateMachine_ThomasTheTank_FSM : MonoBehaviour
{

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

    private void Update()
    {
        if(CurrentState == null)
        {
            CurrentState = states.Values.First();
        }
        else
        {
            var nextState = CurrentState.StateUpdate();

            if(nextState != null && nextState != CurrentState.GetType())
            {
                SwitchToState(nextState);
            }
        }

    }

    void SwitchToState(Type nextState)
    {
        CurrentState.StateExit();
        CurrentState = states[nextState];
        CurrentState.StateEnter();
    }
}
