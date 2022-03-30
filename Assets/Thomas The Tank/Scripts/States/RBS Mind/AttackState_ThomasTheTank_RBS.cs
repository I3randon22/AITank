using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackState_ThomasTheTank_RBS : BaseState_ThomasTheTank_FSM
{
    private SmartTank_ThomasTheTank_RBS smartTank;
    private GameObject TargetLocationRotation;

    public AttackState_ThomasTheTank_RBS(SmartTank_ThomasTheTank_RBS smartTank)
    {
        this.smartTank = smartTank;
    }

    public override Type StateEnter()
    {
        smartTank.stats["attackState"] = true; // add this on every state
        return null;
    }

    public override Type StateExit()
    {
        smartTank.stats["attackState"] = false; // add this on every state
        return null;
    }

    public override Type StateUpdate()
    {
        // Health Check -------------------------------------------------------------
        smartTank.CheckHealth();
        if (smartTank.lowHealth)
        {
            return typeof(EscapeState_ThomasTheTank_RBS); // changes the state to chase
        }
        // --------------------------------------------------------------------------

        if (smartTank.targetTanksFound.Count > 0 && smartTank.targetTanksFound.First().Key != null)
        {
            smartTank.targetTankPosition = smartTank.targetTanksFound.First().Key;
            if (smartTank.targetTankPosition != null)
            {



                if (Vector3.Distance(smartTank.transform.position, smartTank.targetTankPosition.transform.position) < 25f)
                {
                    bool NextPoint = true;
                    if (smartTank.firing)
                    {
                        /*
                        int CirlePoint = 0; // which point of the circle do i follow

                        //Creates the circle around the tank ---------------------------------------------------------------------
                        TargetLocationRotation.transform.position = new Vector3(
                                smartTank.targetTankPosition.transform.position.x + 15 * Mathf.Cos(2 * Mathf.PI * CirlePoint / 8),
                                smartTank.targetTankPosition.transform.position.y,
                                smartTank.targetTankPosition.transform.position.z + 15 * Mathf.Sin(2 * Mathf.PI * CirlePoint / 8));
                        //--------------------------------------------------------------------------------------------------------

                        Debug.Log(TargetLocationRotation);
                        smartTank.OrbitTank(TargetLocationRotation);

                        if (CirlePoint > 8)
                            CirlePoint = 0;
                        else
                        {
                            if (NextPoint)
                            {
                                CirlePoint += 1;
                                NextPoint = false;
                                Debug.Log(TargetLocationRotation);
                            }
                        }*/


                    }
                    else
                    {
                        smartTank.ShootTank();
                        NextPoint = true;
                    }

                    if (smartTank.stats["lowHealth"] == true)
                    {
                        return typeof(EscapeState_ThomasTheTank_RBS);
                    }

                    return null;
                }
                else
                {
                    return typeof(ChaseState_ThomasTheTank_RBS);
                }

            }
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
