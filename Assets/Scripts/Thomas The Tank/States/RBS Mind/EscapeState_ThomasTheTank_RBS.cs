using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeState_ThomasTheTank_RBS : BaseState_ThomasTheTank_FSM
{
    private SmartTank_ThomasTheTank_RBS smartTank;

    public EscapeState_ThomasTheTank_RBS(SmartTank_ThomasTheTank_RBS smartTank)
    {
        this.smartTank = smartTank;
    }

    public override Type StateEnter()
    {
        smartTank.stats["escapeState"] = true; // add this on every state
        return null;
    }

    public override Type StateExit()
    {
        smartTank.stats["escapeState"] = false; // add this on every state
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

        foreach (var item in smartTank.rules.GetRules)
        {
            if (item.CheckRule(smartTank.stats) != null)
            {
                return item.CheckRule(smartTank.stats);
            }
        }

        return null;
    }
}