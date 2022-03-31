using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class SmartTank_ThomasTheTank_RBS : AITank
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

    //cant use start or update, use below instead
    public override void AITankStart()
    {
        InitFSM();
        body = GetComponent<Rigidbody>();
        targetPrefab = Instantiate<GameObject>(targetPrefab);
        Theme = GameObject.Find("ThomasTheTank").GetComponentInParent<AudioSource>();

        stats.Add("lowHealth", lowHealth);
        stats.Add("targetSpotted", false);
        stats.Add("targetReached", false);
        stats.Add("escapeState", false);
        stats.Add("chaseState", false);
        stats.Add("patrolState", false);
        stats.Add("attackState", false);

        rules.AddRule(new Rule("targetSpotted", "lowHealth", typeof(PatrolState_ThomasTheTank_RBS), Rule.Predicate.nAnd)); // if target hasnt been spotted and we arent on low health, go to patrol state
        rules.AddRule(new Rule("chaseState", "lowHealth", typeof(EscapeState_ThomasTheTank_RBS), Rule.Predicate.And));
        rules.AddRule(new Rule("targetReached", "targetSpotted", typeof(AttackState_ThomasTheTank_RBS), Rule.Predicate.nAnd));
        rules.AddRule(new Rule("targetSpotted", "patrolState", typeof(ChaseState_ThomasTheTank_RBS), Rule.Predicate.And));
    }

    void InitFSM()
    {
        //creates a dictionary of all states 
        Dictionary<Type, BaseState_ThomasTheTank_FSM> states = new Dictionary<Type, BaseState_ThomasTheTank_FSM>();
        //Add all states like this to the dictionary
        states.Add(typeof(PatrolState_ThomasTheTank_RBS), new PatrolState_ThomasTheTank_RBS(this));
        states.Add(typeof(AttackState_ThomasTheTank_RBS), new AttackState_ThomasTheTank_RBS(this));
        states.Add(typeof(ChaseState_ThomasTheTank_RBS), new ChaseState_ThomasTheTank_RBS(this));
        states.Add(typeof(EscapeState_ThomasTheTank_RBS), new EscapeState_ThomasTheTank_RBS(this));
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

    public void CheckStats()
    {
        stats["lowHealth"] = lowHealth;

        if (GetHealthLevel < 50)
        {
            lowHealth = true;
        }
        else
        {
            lowHealth = false;
        }


        if (GetFuelLevel < 30)
        {
            lowFuel = true;
        }
        else
        {
            lowFuel = false;
        }

        
        if(GetAmmoLevel < 5)
        {
            lowAmmo = true;
        }
        else
        {
            lowAmmo = false;
        }
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
         firing = IsFiring;    
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
}
