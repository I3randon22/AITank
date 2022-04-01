using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackState_ThomasTheTank_BTS : BaseState_ThomasTheTank_FSM
{
    private SmartTank_ThomasTheTank_BTS smartTank;
    private Vector3 TargetLocationRotation;
    float currentTime = 0;
    bool NextPoint = true;
    int CirlePoint = 0;
    
    public AttackState_ThomasTheTank_BTS(SmartTank_ThomasTheTank_BTS smartTank)
    {
        this.smartTank = smartTank;
    }

    public override Type StateEnter()
    {
        smartTank.stats["attackState"] = true; // add this on every state
        currentTime = 0;
        NextPoint = true;
        CirlePoint = 0;
        return null;
    }

    public override Type StateExit()
    {
        smartTank.stats["attackState"] = false; // add this on every state
        return null;
    }

    

    public override Type StateUpdate()
    {
       
        // Stats Check -------------------------------------------------------------
        smartTank.CheckStats();
        if (smartTank.lowHealth || smartTank.lowFuel || smartTank.lowAmmo)
        {
            return typeof(EscapeState_ThomasTheTank_BTS); // changes the state to chase
        }

        if (smartTank.targetTanksFound.Count > 0 && smartTank.targetTanksFound.First().Key != null)
        {
            smartTank.targetTankPosition = smartTank.targetTanksFound.First().Key;
            if (smartTank.targetTankPosition != null)
            {
                if (Vector3.Distance(smartTank.transform.position, smartTank.targetTankPosition.transform.position) < 35f)
                {
                    
                    currentTime -= Time.deltaTime;
                    if (currentTime <= 0)
                    {
                        NextPoint = true;
                        smartTank.ShootAt(smartTank.targetTankPosition);
                        currentTime = 3f;
                    }
                    else
                    {
                        // which point of the circle do i follow

                        Debug.Log(CirlePoint);
                        if (CirlePoint > 8)
                            CirlePoint = 0;

                        if (NextPoint)
                        {
                            NextPoint = false;
                            //Creates the circle around the tank ---------------------------------------------------------------------
                            TargetLocationRotation = new Vector3(
                                smartTank.targetTankPosition.transform.position.x + 20 * Mathf.Cos(2 * Mathf.PI * CirlePoint / 8),
                                smartTank.targetTankPosition.transform.position.y,
                                smartTank.targetTankPosition.transform.position.z + 20 * Mathf.Sin(2 * Mathf.PI * CirlePoint / 8));
                            //--------------------------------------------------------------------------------------------------------

                            CirlePoint += 1;
                            
                        }

                        smartTank.OrbitTank(TargetLocationRotation);
                    }


                    return null;
                }
                else
                {          
                    return typeof(ChaseState_ThomasTheTank_BTS);
                }

            }
        }
        else if (smartTank.basesFound.Count > 0)
        {
            smartTank.basePosition = smartTank.basesFound.First().Key;
            if (smartTank.basePosition != null)
            {
                Debug.Log("Shoot");
                //go close to it and fire
                if (Vector3.Distance(smartTank.transform.position, smartTank.basePosition.transform.position) < 35f)
                {
                    if(smartTank.basesFound.First().Key != null)
                        smartTank.ShootAt(smartTank.basePosition);
                }
                else
                {
                    smartTank.GoToLocation(smartTank.basePosition);
                }
            }
        }
        else
        {
            smartTank.stats["targetSpotted"] = false;
            smartTank.stats["targetReached"] = false;
            return typeof(PatrolState_ThomasTheTank_BTS);
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
