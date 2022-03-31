using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackState_ThomasTheTank_FSM : BaseState_ThomasTheTank_FSM
{
    private MehSmartTank_ThomasTheTank_FSM smartTank;
    private GameObject TargetLocationRotation;

    public AttackState_ThomasTheTank_FSM(MehSmartTank_ThomasTheTank_FSM smartTank)
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

        if (smartTank.targetTanksFound.Count > 0 && smartTank.targetTanksFound.First().Key != null)
        {
            smartTank.targetTankPosition = smartTank.targetTanksFound.First().Key;
            if (smartTank.targetTankPosition != null)
            {

                if (Vector3.Distance(smartTank.transform.position, smartTank.targetTankPosition.transform.position) < 25f)
                {
                    smartTank.ShootTank();
                }
                else
                {
                    smartTank.GoToPoint(smartTank.targetTankPosition);
                }



            }
        }
        else
        {
            return typeof(ChaseState_ThomasTheTank_FSM);
        }
        return null;
    }
}
