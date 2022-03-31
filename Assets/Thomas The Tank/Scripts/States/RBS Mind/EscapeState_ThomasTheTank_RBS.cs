using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeState_ThomasTheTank_RBS : BaseState_ThomasTheTank_FSM
{
    private SmartTank_ThomasTheTank_RBS smartTank;

    public EscapeState_ThomasTheTank_RBS(SmartTank_ThomasTheTank_RBS smartTank)
    {
        this.smartTank = smartTank;
    }

    public override Type StateEnter()
    {
        smartTank.stats["escapeState"] = true; // add this on every state
        return null;
    }

    public override Type StateExit()
    {
        smartTank.stats["escapeState"] = false; // add this on every state
        return null;
    }

    public override Type StateUpdate()
    {
        /**********************EXAMPLE*********************/
        /* //If target becomes closer than 3 then change state to chase state
        if(Vector3.Distance(smartTank.transform.position, smartTank.targetTankPosition.transform.position) < 3f)
        {
            return typeof(ChaseState);
        }
        else
        {
            RandomPatrol();
            return null;
        }
        */

        //seen consumables added to consumablesFound
        smartTank.SyncConsumablesFound();

        //if the tank has sufficient resources, return to patrol state
        if ((smartTank.stats["lowHealth"] == false) && (smartTank.stats["lowAmmo"] == false) && (smartTank.stats["lowFuel"] == false))
        {
            return typeof(PatrolState_ThomasTheTank_FSM);
        }
        else
        {
            if (smartTank.consumablesFound.Count > 0)
            {

                //goes through all the consumables the tank has seen, checks their tags to see if its health ammo or fuel
                //tank goes to which consumable it needs most, ie if its low on health and knows of a health kit, it goes to the health kit

                for (int i = 0; i < smartTank.consumablesFound.Count; i++)
                {
                    if (smartTank.stats["lowHealth"] == true && smartTank.consumablesFound.ElementAt(i).Key.tag == "Health")
                    {
                        Debug.Log("Going to Health");
                        smartTank.consumablePosition = smartTank.consumablesFound.ElementAt(i).Key;
                        smartTank.FollowToPoint(smartTank.consumablePosition);
                    }
                    else if (smartTank.stats["lowAmmo"] == true && smartTank.consumablesFound.ElementAt(i).Key.tag == "Ammo")
                    {
                        Debug.Log("Going to Ammo");
                        smartTank.consumablePosition = smartTank.consumablesFound.ElementAt(i).Key;
                        smartTank.FollowToPoint(smartTank.consumablePosition);
                    }
                    else if (smartTank.stats["lowFuel"] == true && smartTank.consumablesFound.ElementAt(i).Key.tag == "Fuel")
                    {
                        Debug.Log("Going to Fuel");
                        smartTank.consumablePosition = smartTank.consumablesFound.ElementAt(i).Key;
                        smartTank.FollowToPoint(smartTank.consumablePosition);
                    }
                    else
                    {
                        smartTank.RandomRoam();
                        Debug.Log("Roaming for right resource");
                    }

                }
            }
            else
            {
                //random roam if no consumables found
                smartTank.RandomRoam();
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
