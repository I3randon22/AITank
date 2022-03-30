using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeState_ThomasTheTank_FSM : BaseState_ThomasTheTank_FSM
{
    private MehSmartTank_ThomasTheTank_FSM smartTank;

    public EscapeState_ThomasTheTank_FSM(MehSmartTank_ThomasTheTank_FSM smartTank)
    {
        this.smartTank = smartTank;
    }

    public override Type StateEnter()
    {
        return null;
    }

    public override Type StateExit()
    {
        return null;
    }

    public override Type StateUpdate()
    {
        /**********************EXAMPLE*********************/
        /* //If target becomes closer than 3 then change state to chase state
        if(Vector3.Distance(smartTank.transform.position, smartTank.targetTankPosition.transform.position) < 3f)
        {
            return typeof(ChaseState);
        }
        else
        {
            RandomPatrol();
            return null;
        }
        */
        return null;
    }
}
