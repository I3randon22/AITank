using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState_ThomasTheTank_FSM : BaseState_ThomasTheTank_FSM
{
    private SmartTank_ThomasTheTank_FSM smartTank;

    public ChaseState_ThomasTheTank_FSM(SmartTank_ThomasTheTank_FSM smartTank)
    {
        this.smartTank = smartTank;
    }

    public override Type StateEnter()
    {
        Debug.Log("ChaseEnter");
        return null;
    }

    public override Type StateExit()
    {
        Debug.Log("ChaseExit");
        return null;
    }

    public override Type StateUpdate()
    {
        
        return null;
    }
}
