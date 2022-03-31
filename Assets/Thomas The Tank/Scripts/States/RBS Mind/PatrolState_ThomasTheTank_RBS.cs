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
        if (smartTank.lowHealth || smartTank.lowFuel || smartTank.lowAmmo)
        {
            return typeof(EscapeState_ThomasTheTank_RBS); // changes the state to chase
        }

        smartTank.SyncDataFound();
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
            //if base if found
            smartTank.basePosition = smartTank.basesFound.First().Key;
            if (smartTank.basePosition != null)
            {
                return typeof(AttackState_ThomasTheTank_RBS);
            }
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

        // Basic Patrol System --------------------------------------------------------------
        smartTank.FollowPoint(); //  basic follows generate point
        currentTime += Time.deltaTime;
        if (currentTime > 10)
        {
            smartTank.GeneratePoint(); //  basic generates random point
            currentTime = 0;
        }

        // custom patrol system --------------------------------------------------------------------
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
