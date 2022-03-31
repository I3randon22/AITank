using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EscapeState_ThomasTheTank_BTS : BaseState_ThomasTheTank_FSM
{
    private SmartTank_ThomasTheTank_BTS smartTank;
    float currentTime;

    public EscapeState_ThomasTheTank_BTS(SmartTank_ThomasTheTank_BTS smartTank)
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
        //seen consumables added to consumablesFound
        smartTank.SyncDataFound();

        smartTank.CheckStats();
        //if the tank has sufficient resources, return to patrol state
        if (smartTank.regenSequence != null && smartTank.regenSequence.Evaluate() == BTNodesStates.SUCCESS)
        {
            if (smartTank.targetTanksFound.Count > 0)
            {
                smartTank.stats["targetSpotted"] = true; // tells the rules system we've found it

                return typeof(ChaseState_ThomasTheTank_BTS); // changes the state to chase
            }
            else
                return typeof(PatrolState_ThomasTheTank_BTS);
        }
        else
        {
            RandomRoam(); //RandomRoam until tank has sufficient resources
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

    private void RandomRoam()
    {
        // Basic Patrol System --------------------------------------------------------------
        smartTank.FollowPoint(); //  basic follows generate point
        currentTime += Time.deltaTime;
        if (currentTime > 10)
        {
            smartTank.GeneratePoint(); //  basic generates random point
            currentTime = 0;
        }
    }
}
