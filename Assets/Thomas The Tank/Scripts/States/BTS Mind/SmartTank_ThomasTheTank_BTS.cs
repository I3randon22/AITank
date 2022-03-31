using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class SmartTank_ThomasTheTank_BTS : AITank
{
    public Dictionary<GameObject, float> targetTanksFound = new Dictionary<GameObject, float>();
    public Dictionary<GameObject, float> consumablesFound = new Dictionary<GameObject, float>();
    public Dictionary<GameObject, float> basesFound = new Dictionary<GameObject, float>();

    public GameObject targetTankPosition;
    public GameObject consumablePosition;
    public GameObject basePosition;

    public GameObject targetPrefab;

    [HideInInspector] float currentTime;
    Rigidbody body;

    public Dictionary<string, bool> stats = new Dictionary<string, bool>();
    public Rules rules = new Rules();

    [HideInInspector] public bool lowHealth = false;
    [HideInInspector] public bool lowFuel = false;
    [HideInInspector] public bool lowAmmo = false;


    [HideInInspector] public bool firing;
    public AudioSource Theme;

    //BT
    public BTAction healthCheck;
    public BTAction ammoCheck;
    public BTAction fuelCheck;
    public BTAction targetFoundCheck;
    public BTAction targetReachedCheck;
    public BTSequence regenSequence; //Health regen

    //cant use start or update, use below instead
    public override void AITankStart()
    {
        InitFSM();
        body = GetComponent<Rigidbody>();
        targetPrefab = Instantiate<GameObject>(targetPrefab);
        Theme = GameObject.Find("ThomasTheTank").GetComponentInParent<AudioSource>();

        //-----------Stats----------//
        stats.Add("lowHealth", lowHealth);
        stats.Add("lowAmmo", lowAmmo);
        stats.Add("lowFuel", lowFuel);
        stats.Add("targetSpotted", false);
        stats.Add("targetReached", false);
        stats.Add("escapeState", false);
        stats.Add("chaseState", false);
        stats.Add("patrolState", false);
        stats.Add("attackState", false);

        //-----------Rules-----------//
        rules.AddRule(new Rule("targetSpotted", "lowHealth", typeof(PatrolState_ThomasTheTank_BTS), Rule.Predicate.nAnd)); // if target hasnt been spotted and we arent on low health, go to patrol state
        rules.AddRule(new Rule("chaseState", "lowHealth", typeof(EscapeState_ThomasTheTank_BTS), Rule.Predicate.And));
        rules.AddRule(new Rule("targetReached", "targetSpotted", typeof(AttackState_ThomasTheTank_BTS), Rule.Predicate.nAnd));
        rules.AddRule(new Rule("targetSpotted", "patrolState", typeof(ChaseState_ThomasTheTank_BTS), Rule.Predicate.And));

        //---------------BT----------------//
        //Create Actions
        healthCheck = new BTAction(HealthCheck);
        ammoCheck = new BTAction(AmmoCheck);
        fuelCheck = new BTAction(FuelCheck);
        targetFoundCheck = new BTAction(TargetFoundCheck);
        targetReachedCheck = new BTAction(TargetReachedCheck);
        //Create Sequence
        regenSequence = new BTSequence(new List<BTBaseNode> { healthCheck, ammoCheck, fuelCheck }); 
    }

    //---------------------------ACTIONS------------------------------------//
    public BTNodesStates HealthCheck()
    {
        if (!stats["lowFuel"]) //if not low on fuel but low on health get health
        {
            //keeps running until success
            if (stats["lowHealth"])
            {
                GrabResource("Health");
                return BTNodesStates.FAILURE;
            }
            else //no longer low health so success and move on
            {
                return BTNodesStates.SUCCESS;
            }
        }

        return BTNodesStates.SUCCESS;
    }

    public BTNodesStates AmmoCheck()
    {
        if (!stats["lowFuel"] || !stats["lowHealth"]) //if not on low fuel or health but low on ammo get ammo
        {
            //keeps running until success
            if (stats["lowAmmo"])
            {
                GrabResource("Ammo");
                return BTNodesStates.FAILURE;
            }
            else //no longer low ammo so success and move on
            {
                return BTNodesStates.SUCCESS;
            }
        }

        return BTNodesStates.SUCCESS;
    }

    public BTNodesStates FuelCheck()
    {
        //keeps running until success
        if (stats["lowFuel"])
        {
            GrabResource("Fuel");
            return BTNodesStates.FAILURE;
        }
        else //no longer low fuel so success and move on
        {
            return BTNodesStates.SUCCESS;
        }
    }

    public BTNodesStates TargetFoundCheck()
    {
        //Check if target is found
        if (stats["targetSpotted"])
        {
            return BTNodesStates.SUCCESS;
        }
        else
        {
            return BTNodesStates.FAILURE;
        }
    }

    public BTNodesStates TargetReachedCheck()
    {
        //Check if reached target
        if (stats["targetReached"])
        {
            return BTNodesStates.SUCCESS;
        }
        else
        {
            return BTNodesStates.FAILURE;
        }
    }

    //-------------------------END OF ACTIONS-----------------------------//


    //Use to grab a resource and go to it
    public void GrabResource(string resourceTag)
    {
        if (consumablesFound.Count > 0 && consumablesFound != null)
        {
            for (int i = 0; i < consumablesFound.Count; i++)
            {
                if (consumablesFound.ElementAt(i).Key.tag == resourceTag)
                {
                    consumablePosition = consumablesFound.ElementAt(i).Key;
                    GoToLocation(consumablePosition);
                }
            }
        }
        else
        {
            Debug.Log("Low on " + resourceTag + "but cannot find any");
        }
    }

    void InitFSM()
    {
        //creates a dictionary of all states 
        Dictionary<Type, BaseState_ThomasTheTank_FSM> states = new Dictionary<Type, BaseState_ThomasTheTank_FSM>();
        //Add all states like this to the dictionary
        states.Add(typeof(PatrolState_ThomasTheTank_BTS), new PatrolState_ThomasTheTank_BTS(this));
        states.Add(typeof(AttackState_ThomasTheTank_BTS), new AttackState_ThomasTheTank_BTS(this));
        states.Add(typeof(ChaseState_ThomasTheTank_BTS), new ChaseState_ThomasTheTank_BTS(this));
        states.Add(typeof(EscapeState_ThomasTheTank_BTS), new EscapeState_ThomasTheTank_BTS(this));
        //Set states
        GetComponent<FiniteStateMachine_ThomasTheTank_FSM>().SetStates(states);
    }
    
    public override void AITankUpdate()
    {
        //Get the targets found from the sensor view
        targetTanksFound = GetAllTargetTanksFound;
        consumablesFound = GetAllConsumablesFound;
        basesFound = GetAllBasesFound;

        
        
        //if low health or ammo, go searching
        if (GetHealthLevel < 50 || GetAmmoLevel < 5)
        {
            if (consumablesFound.Count > 0)
            {
                consumablePosition = consumablesFound.First().Key;
                //FollowPathToPoint(consumablePosition, 1f);
                currentTime += Time.deltaTime;
                if (currentTime > 10)
                {
                    GenerateRandomPoint();
                    currentTime = 0;
                }
            }
            else
            {
                targetTankPosition = null;
                consumablePosition = null;
                basePosition = null;
                //FollowPathToRandomPoint(1f);
            }
        }
        else
        {
            //if there is a target found
            if (targetTanksFound.Count > 0 && targetTanksFound.First().Key != null)
            {
                targetTankPosition = targetTanksFound.First().Key;
                if (targetTankPosition != null)
                {
                    //get closer to target, and fire
                    if (Vector3.Distance(transform.position, targetTankPosition.transform.position) < 25f)
                    {
                        //FireAtPoint(targetTankPosition);
                    }
                    else
                    {
                        //FollowPathToPoint(targetTankPosition, 1f);
                    }
                }
            }
            else if (consumablesFound.Count > 0)
            {
                //if consumables are found, go to it.
                consumablePosition = consumablesFound.First().Key;
                FollowPathToPoint(consumablePosition, 1f);
            }
            else if (basesFound.Count > 0)
            {
                //if base if found
                basePosition = basesFound.First().Key;
                if (basePosition != null)
                {
                    //go close to it and fire
                    if (Vector3.Distance(transform.position, basePosition.transform.position) < 25f)
                    {
                        //FireAtPoint(basePosition);
                    }
                    else
                    {
                        //FollowPathToPoint(basePosition, 1f);
                    }
                }
            }
            else
            {
                //searching
                targetTankPosition = null;
                consumablePosition = null;
                basePosition = null;
                //FollowPathToRandomPoint(1f);
                currentTime += Time.deltaTime;
                if (currentTime > 10)
                {
                    GenerateRandomPoint();
                    currentTime = 0;
                }
            }
        }
    }

    public override void AIOnCollisionEnter(Collision collision)
    {
      
    }

   
    // Chase Script ------------------------------------------------------------------------------- 
    public void ChaseTank()
    {
        if (targetTankPosition != null)
        {
            FollowPathToPoint(targetTankPosition, 1f);
        }
    }
    // Attack Script ------------------------------------------------------------------------------
    public void ShootAt(GameObject Location)
    {
         FireAtPoint(Location);
    }

    public void OrbitTank(Vector3 NextLocation)
    {
        targetPrefab.transform.position = NextLocation;
        FollowPathToPoint(targetPrefab, 1f);
    }

    // Patrol Script --------------------------------------------------------------------------------
    public void GeneratePoint()
    {
        GenerateRandomPoint();
    }

    public void FollowPoint()
    {
        FollowPathToRandomPoint(1f);
    }

    public void SyncDataFound()
    {
        targetTanksFound = GetAllTargetTanksFound;
        consumablesFound = GetAllConsumablesFound;
        basesFound = GetAllBasesFound;
    }

    public void GoToLocation(GameObject Location)
    {
        FollowPathToPoint(Location, 1f);
    }

    // Escape Script --------------------------------------------------------------------------------
    public void CheckStats()
    {
        stats["lowHealth"] = lowHealth;
        stats["lowFuel"] = lowFuel;
        stats["lowAmmo"] = lowAmmo;

        firing = IsFiring;

        if (GetHealthLevel < 50)
        {
            lowHealth = true;
            //Debug.Log("Health At: " + GetHealthLevel);
        }
        else
        {
            lowHealth = false;
        }


        if (GetFuelLevel < 30)
        {
            lowFuel = true;
            //Debug.Log("Fuel At: " + GetFuelLevel);
        }
        else
        {
            lowFuel = false;
        }

        
        if(GetAmmoLevel < 5)
        {
            lowAmmo = true;
            //Debug.Log("Ammo At: " + GetAmmoLevel);
        }
        else
        {
            lowAmmo = false;
        }
    }
}
