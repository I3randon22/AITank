using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
        smartTank.CheckStats();
        if (smartTank.lowHealth || smartTank.lowAmmo || smartTank.lowFuel)
        {
            return typeof(EscapeState_ThomasTheTank_FSM); // changes the state to chase
        }
        // --------------------------------------------------------------------------

        smartTank.SyncDataFound();
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
        if (smartTank.consumablesFound.Count > 0)
        {
            smartTank.consumablePosition = smartTank.consumablesFound.First().Key;
            smartTank.GoToPoint(smartTank.consumablePosition);
        }
        else
        {
            RandomRoam();
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
