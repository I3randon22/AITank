using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChaseState_ThomasTheTank_BTS : BaseState_ThomasTheTank_FSM
{
    private SmartTank_ThomasTheTank_BTS smartTank;

    public ChaseState_ThomasTheTank_BTS(SmartTank_ThomasTheTank_BTS smartTank)
    {
        this.smartTank = smartTank;
    }

    public override Type StateEnter()
    {
        smartTank.stats["chaseState"] = true; // add this on every state
        return null;
    }

    public override Type StateExit()
    {
        smartTank.stats["chaseState"] = false; // add this on every state
        return null;
    }

    public override Type StateUpdate()
    {
        // Stats Check -------------------------------------------------------------
        smartTank.CheckStats();
        if (smartTank.lowHealth || smartTank.lowFuel)
        {
            return typeof(EscapeState_ThomasTheTank_BTS); // changes the state to chase
        }

        smartTank.targetTankPosition = smartTank.targetTanksFound.First().Key;
        if (Vector3.Distance(smartTank.transform.position, smartTank.targetTankPosition.transform.position) < 30f)
        {
            smartTank.stats["targetReached"] = true; // changing the rules to found
            return (typeof(AttackState_ThomasTheTank_BTS));
        }
        else
        {
            smartTank.stats["targetReached"] = false;
            smartTank.ChaseTank();          
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
