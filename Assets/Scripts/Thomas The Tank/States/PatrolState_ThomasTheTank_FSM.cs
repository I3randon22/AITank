using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState_ThomasTheTank_FSM : BaseState_ThomasTheTank_FSM
{
    private SmartTank_ThomasTheTank_FSM smartTank;
    float t, searchT;
    int rotation;

    public PatrolState_ThomasTheTank_FSM(SmartTank_ThomasTheTank_FSM smartTank)
    {
        this.smartTank = smartTank;
    }

    public override Type StateEnter()
    {
        Debug.Log("PatrolEnter");
        return null;
    }

    public override Type StateExit()
    {
        Debug.Log("PatrolExit");
        return null;
    }

    public override Type StateUpdate()
    {
        //if tank sees other tank go to chase state
        if (smartTank.targetTankPosition != null && smartTank.targetTanksFound.Count > 0)
        {
            return (typeof(ChaseState_ThomasTheTank_FSM));

            /* Put in Attack state*
            if (Vector3.Distance(smartTank.transform.position, smartTank.targetTankPosition.transform.position) < 25f)
            {
                Debug.Log("Attack");
            }
            */
        }
        else
        {
            //Goto random points(patrol) then stop and look around and scan for a bit
            searchT += Time.deltaTime;

            if(searchT < 10)
            {
                smartTank.SearchRandomPoint();
            }
            else
            {
                t += Time.deltaTime;

                if (t > 0.2)
                {
                    t = 0;
                    smartTank.TurretSpin(rotation);
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
        return null;
    }
}
