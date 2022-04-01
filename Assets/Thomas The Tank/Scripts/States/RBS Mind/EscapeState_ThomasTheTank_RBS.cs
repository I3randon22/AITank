using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EscapeState_ThomasTheTank_RBS : BaseState_ThomasTheTank_FSM
{
    private SmartTank_ThomasTheTank_RBS smartTank;
    float currentTime;
    bool hasRetreated;
    Vector3 area;

    public EscapeState_ThomasTheTank_RBS(SmartTank_ThomasTheTank_RBS smartTank)
    {
        this.smartTank = smartTank;
    }

    public override Type StateEnter()
    {
        hasRetreated = false;
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
        //seen consumables added to consumablesFound
        smartTank.SyncDataFound();

        smartTank.CheckStats();


        //if the tank has sufficient resources, return to patrol state
        if ((smartTank.lowHealth == false) && (smartTank.lowFuel == false) && (smartTank.lowAmmo == false))
        {
            if (smartTank.targetTanksFound.Count > 0)
            {
                smartTank.stats["targetSpotted"] = true; // tells the rules system we've found it

                return typeof(ChaseState_ThomasTheTank_RBS); // changes the state to chase
            }
            else
                return typeof(PatrolState_ThomasTheTank_FSM);
        }
        else
        {
            if (smartTank.consumablesFound.Count > 0 && smartTank.consumablesFound != null)
            {
                //goes through all the consumables the tank has seen, checks their tags to see if its health ammo or fuel
                //tank goes to which consumable it needs most, ie if its low on health and knows of a health kit, it goes to the health kit

                for (int i = 0; i < smartTank.consumablesFound.Count; i++)
                {
                    if(smartTank.lowHealth == true && smartTank.lowAmmo == true && smartTank.lowFuel == true)
                    {
                        if(smartTank.consumablesFound.ElementAt(i).Key.tag == "Health" || smartTank.consumablesFound.ElementAt(i).Key.tag == "Fuel")
                        {
                            Debug.Log("Going to Health/Fuel");
                            smartTank.consumablePosition = smartTank.consumablesFound.ElementAt(i).Key;
                            smartTank.GoToLocation(smartTank.consumablePosition);
                        }

                    }
                    else if (smartTank.lowHealth == true && (smartTank.consumablesFound.ElementAt(i).Key.tag == "Health" || smartTank.consumablesFound.ElementAt(i).Key.tag == "Fuel"))
                    {
                        Debug.Log("Going to Health");
                        smartTank.consumablePosition = smartTank.consumablesFound.ElementAt(i).Key;
                        smartTank.GoToLocation(smartTank.consumablePosition);
                    }
                    else if (smartTank.lowAmmo == true && (smartTank.consumablesFound.ElementAt(i).Key.tag == "Ammo" || smartTank.consumablesFound.ElementAt(i).Key.tag == "Health" || smartTank.consumablesFound.ElementAt(i).Key.tag == "Fuel"))
                    {
                        Debug.Log("Going to Ammo");
                        smartTank.consumablePosition = smartTank.consumablesFound.ElementAt(i).Key;
                        smartTank.GoToLocation(smartTank.consumablePosition);
                    }
                    else if (smartTank.lowFuel == true && (smartTank.consumablesFound.ElementAt(i).Key.tag == "Fuel" || smartTank.consumablesFound.ElementAt(i).Key.tag == "Health"))
                    {
                        Debug.Log("Going to Fuel");
                        smartTank.consumablePosition = smartTank.consumablesFound.ElementAt(i).Key;
                        smartTank.GoToLocation(smartTank.consumablePosition);
                    }
                    else
                    {
                        //RandomRoam();
                    }

                }
            }
            else
            {
                if(hasRetreated)
                {
                    Debug.Log("Roaming...");
                    RandomRoam();
                }
                else
                {
                    Debug.Log("Retreating...");
                    area = new Vector3(80, 0, 80);
                    smartTank.RetreatToArea(area);

                    if(Vector3.Distance(smartTank.transform.position, area) < 20)
                    {
                        hasRetreated = true;
                    }
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
