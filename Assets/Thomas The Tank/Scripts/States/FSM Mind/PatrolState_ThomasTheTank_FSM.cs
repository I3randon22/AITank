using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState_ThomasTheTank_FSM : BaseState_ThomasTheTank_FSM
{
    private MehSmartTank_ThomasTheTank_FSM smartTank;

    float currentTime;

    float t, searchT;
    int rotation;


    public PatrolState_ThomasTheTank_FSM(MehSmartTank_ThomasTheTank_FSM smartTank)
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
        // Health Check -------------------------------------------------------------
        smartTank.CheckHealth();
        if (smartTank.lowHealth)
        {
            return typeof(EscapeState_ThomasTheTank_FSM); // changes the state to chase
        }
        // --------------------------------------------------------------------------

        smartTank.SyncTanksFound();
        //if tank sees other tank go to chase state
        if (smartTank.targetTanksFound.Count > 0)
        {
            // Music System --------------------------------------------------------
            if (!smartTank.Theme.isPlaying && smartTank.Theme != null)
            {
                GameObject.Find("Music").GetComponentInParent<AudioSource>().Stop();
                smartTank.Theme.Play();
            }
            // ---------------------------------------------------------------------    

            return typeof(ChaseState_ThomasTheTank_FSM); // changes the state to chase
        }
        else
        {
            RandomPatrol();
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

        if (searchT < 10)
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

        if (searchT >= 20)
        {
            searchT = 0;
        }

    }
}
