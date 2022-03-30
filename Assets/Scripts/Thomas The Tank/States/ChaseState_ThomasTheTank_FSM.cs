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
        smartTank.stats["chaseState"] = true; // add this on every state
        Debug.Log("ChaseEnter");
        return null;
    }

    public override Type StateExit()
    {
        smartTank.stats["chaseState"] = false; // add this on every state
        Debug.Log("ChaseExit");
        return null;
    }

    public override Type StateUpdate()
    {
        foreach (var item in smartTank.rules.GetRules)
        {
            if (item.CheckRule(smartTank.stats) != null)
            {
                return item.CheckRule(smartTank.stats);
            }
        }

        if (Vector3.Distance(smartTank.transform.position, smartTank.targetTankPosition.transform.position) < 25f)
        {
            return (typeof(AttackState_ThomasTheTank_FSM));
        }
        else
        {
            smartTank.ChaseTank();
            return null;
        }
    }
}
