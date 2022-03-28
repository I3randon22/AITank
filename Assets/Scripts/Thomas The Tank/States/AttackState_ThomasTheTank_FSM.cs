using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState_ThomasTheTank_FSM : BaseState_ThomasTheTank_FSM
{
    private SmartTank_ThomasTheTank_FSM smartTank;

    public AttackState_ThomasTheTank_FSM(SmartTank_ThomasTheTank_FSM smartTank)
    {
        this.smartTank = smartTank;
    }

    public override Type StateEnter()
    {
        smartTank.stats["attackState"] = true; // add this on every state
        return null;
    }

    public override Type StateExit()
    {
        smartTank.stats["attackState"] = false; // add this on every state
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



        if (Vector3.Distance(smartTank.transform.position, smartTank.targetTankPosition.transform.position) < 25f)
        {
            //smartTank.FireAtPoint(smartTank.targetTankPosition);
        }
        else
        {
            //smartTank.FollowPathToPoint(smartTank.targetTankPosition, 1f);
        }

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
