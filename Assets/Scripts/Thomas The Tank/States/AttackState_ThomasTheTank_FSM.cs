using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState_ThomasTheTank_FSM : BaseState_ThomasTheTank_FSM
{
    private SmartTank_ThomasTheTank_FSM smartTank;
    private GameObject TargetLocationRotation;

    public AttackState_ThomasTheTank_FSM(SmartTank_ThomasTheTank_FSM smartTank)
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
        foreach (var item in smartTank.rules.GetRules)
        {
            if (item.CheckRule(smartTank.stats) != null)
            {
                return item.CheckRule(smartTank.stats);
            }
        }

        if (Vector3.Distance(smartTank.transform.position, smartTank.targetTankPosition.transform.position) < 25f)
        {
            bool NextPoint = true;
            if (smartTank.firing)
            {
                int CirlePoint = 0; // which point of the circle do i follow

                //Creates the circle around the tank ---------------------------------------------------------------------
                TargetLocationRotation.transform.position = new Vector3(
                        smartTank.targetTankPosition.transform.position.x + 15 * Mathf.Cos(2 * Mathf.PI * CirlePoint / 8),
                        smartTank.targetTankPosition.transform.position.y,
                        smartTank.targetTankPosition.transform.position.z + 15 * Mathf.Sin(2 * Mathf.PI * CirlePoint / 8));
                //--------------------------------------------------------------------------------------------------------

                if (CirlePoint > 8)
                    CirlePoint = 0;
                else
                {
                    if(NextPoint)
                    {
                        CirlePoint += 1;
                        NextPoint = false;
                        Debug.Log(TargetLocationRotation);
                    }                    
                }
                    
            }
            else
            {
                //smartTank.FireAtPoint(smartTank.targetTankPosition);
                NextPoint = true;
            }

            return null;
        }
        else
        {
            return (typeof(ChaseState_ThomasTheTank_FSM));
        }
    }
}
