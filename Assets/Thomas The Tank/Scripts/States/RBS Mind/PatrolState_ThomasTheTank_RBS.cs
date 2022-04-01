using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PatrolState_ThomasTheTank_RBS : BaseState_ThomasTheTank_FSM
{
    private SmartTank_ThomasTheTank_RBS smartTank;

    float currentTime;

    float t, searchT;
    int rotation;


    public PatrolState_ThomasTheTank_RBS(SmartTank_ThomasTheTank_RBS smartTank)
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
        // Stats Check -------------------------------------------------------------
        smartTank.CheckStats();
        smartTank.SyncDataFound();

        if (smartTank.lowHealth || smartTank.lowFuel || smartTank.lowAmmo)
        {
            return typeof(EscapeState_ThomasTheTank_RBS); // changes the state to chase
        }

      
        //if tank sees other tank go to chase state
        if (smartTank.targetTanksFound.Count > 0)
        {
            smartTank.stats["targetSpotted"] = true; // tells the rules system we've found it

            // Music System --------------------------------------------------------
            if(!smartTank.Theme.isPlaying && smartTank.Theme != null)
            {
                GameObject.Find("Music").GetComponentInParent<AudioSource>().Stop();
                smartTank.Theme.Play();
            }
            // ---------------------------------------------------------------------    

            return typeof(ChaseState_ThomasTheTank_RBS); // changes the state to chase
        }
        else if (smartTank.consumablesFound.Count > 0)
        {
            //if consumables are found, go to it.
            smartTank.consumablePosition = smartTank.consumablesFound.First().Key;
            smartTank.GoToLocation(smartTank.consumablePosition);
        }
        else if (smartTank.basesFound.Count > 0)
        {
            //if base is found
            smartTank.basePosition = smartTank.basesFound.First().Key;
            if (smartTank.basePosition != null)
            {
                return typeof(AttackState_ThomasTheTank_RBS);
            }
        }
        else
        {
            CustomPatrol();
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

    private void CustomPatrol() 
    {
        smartTank.targetTankPosition = null;
        smartTank.consumablePosition = null;
        smartTank.basePosition = null;

        // custom patrol system --------------------------------------------------------------------
        //Goto random points(patrol) then stop and look around and scan for a bit
        searchT += Time.deltaTime;

        if(searchT < 10)
        {
            smartTank.SearchRandomPoint();
        }
        else
        {
            t += Time.deltaTime;

            if (t > 0.2)
            {
                t = 0;
                smartTank.TurretSpin(rotation);

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
