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
        smartTank.CheckHealth();
        if (smartTank.lowHealth)
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

                    if (smartTank.lowHealth == true)
                    {
                        return typeof(EscapeState_ThomasTheTank_FSM);
                    }

                    return null;
                }
                else
                {
                    return typeof(ChaseState_ThomasTheTank_FSM);
                }

            }
        }
        return null;
    }
}
