using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EscapeState_ThomasTheTank_FSM : BaseState_ThomasTheTank_FSM
{
    private MehSmartTank_ThomasTheTank_FSM smartTank;
    float currentTime;

    public EscapeState_ThomasTheTank_FSM(MehSmartTank_ThomasTheTank_FSM smartTank)
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
       

        smartTank.CheckStats();
        smartTank.SyncDataFound();

        //if sufficient resources, go to patrol
        if(smartTank.lowHealth == false && smartTank.lowAmmo == false && smartTank.lowFuel == false)
        {
            return typeof(PatrolState_ThomasTheTank_FSM);
        }
        else
        {
            //if found consumables, go to it
            if(smartTank.consumablesFound.Count > 0)
            {
                smartTank.consumablePosition = smartTank.consumablesFound.First().Key;
                smartTank.GoToPoint(smartTank.consumablePosition);
            }
            else
            {
                //if no consumbales found, random roam
                RandomRoam();
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
