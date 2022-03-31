using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PatrolState_ThomasTheTank_BTS : BaseState_ThomasTheTank_FSM
{
    private SmartTank_ThomasTheTank_BTS smartTank;

    float currentTime;

    float t, searchT;
    int rotation;


    public PatrolState_ThomasTheTank_BTS(SmartTank_ThomasTheTank_BTS smartTank)
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
        //If we dont need health or ammo or fuel continue patrolling  
        if(smartTank.regenSequence != null && smartTank.regenSequence.Evaluate() == BTNodesStates.SUCCESS)
        {
            // Stats Check -------------------------------------------------------No Longer Needed?
            /*
            smartTank.CheckStats();
            if (smartTank.lowHealth || smartTank.lowFuel || smartTank.lowAmmo)
            {
                return typeof(EscapeState_ThomasTheTank_BTS); // changes the state to chase
            }
            */

            smartTank.SyncDataFound();

            //if tank sees other tank go to chase state
            if (smartTank.targetTanksFound.Count > 0)
            {
                smartTank.stats["targetSpotted"] = true; // tells the rules system we've found it

                // Music System --------------------------------------------------------
                if (!smartTank.Theme.isPlaying && smartTank.Theme != null)
                {
                    GameObject.Find("Music").GetComponentInParent<AudioSource>().Stop();
                    smartTank.Theme.Play();
                }
                // ---------------------------------------------------------------------    

                return typeof(ChaseState_ThomasTheTank_BTS); // changes the state to chase
            }
            else if (smartTank.consumablesFound.Count > 0) //Store location, save for later?
            {
                //if consumables are found, go to it.
                smartTank.consumablePosition = smartTank.consumablesFound.First().Key;
                smartTank.GoToLocation(smartTank.consumablePosition);
                
            }
            else if (smartTank.basesFound.Count > 0) //if base found attack it
            {
                //if base if found
                smartTank.basePosition = smartTank.basesFound.First().Key;
                if (smartTank.basePosition != null)
                {
                    return typeof(AttackState_ThomasTheTank_BTS);
                }
            }

            RandomPatrol();
            foreach (var item in smartTank.rules.GetRules)
            {
                if (item.CheckRule(smartTank.stats) != null)
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
