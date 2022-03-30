using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState_ThomasTheTank_FSM : BaseState_ThomasTheTank_FSM
{
    private SmartTank_ThomasTheTank_FSM smartTank;

    float currentTime;

    float t, searchT;
    int rotation;


    public PatrolState_ThomasTheTank_FSM(SmartTank_ThomasTheTank_FSM smartTank)
    {
        this.smartTank = smartTank;
    }

    public override Type StateEnter()
    {

        smartTank.stats["patrolState"] = true; // add this on every state
        return null;
    }

    public override Type StateExit()
    {

        smartTank.stats["patrolState"] = false; // add this on every state
        return null;
    }

    public override Type StateUpdate()
    {
        smartTank.SyncTanksFound();
        //if tank sees other tank go to chase state
        if (smartTank.targetTanksFound.Count > 0)
        {
            smartTank.stats["targetSpotted"] = true;
            return typeof(ChaseState_ThomasTheTank_FSM);
        }
        else
        {
            RandomPatrol();
            foreach (var item in smartTank.rules.GetRules)
            {
                if(item.CheckRule(smartTank.stats) != null)
                {
                    return item.CheckRule(smartTank.stats);
                }
            }
            
        }

        return null;
    }

    private void RandomPatrol()
    {
        smartTank.targetTankPosition = null;
        smartTank.consumablePosition = null;
        smartTank.basePosition = null;
        smartTank.FollowPoint(); // follows generate point
        currentTime += Time.deltaTime;
        if (currentTime > 10)
        {
            smartTank.GeneratePoint(); // generates random point
            currentTime = 0;
        }
            //Goto random points(patrol) then stop and look around and scan for a bit
            searchT += Time.deltaTime;

            if(searchT < 10)
            {
                //smartTank.SearchRandomPoint();
            }
            else
            {
                t += Time.deltaTime;

                if (t > 0.2)
                {
                    t = 0;
                    //smartTank.TurretSpin(rotation);
                    //Debug.Log(rotation);
                    rotation += 1;

                    if (rotation > 8)
                    {
                        rotation = 1;
                    }
                }
            }

            if(searchT >= 20)
            {
                searchT = 0;
            }

    }
}
