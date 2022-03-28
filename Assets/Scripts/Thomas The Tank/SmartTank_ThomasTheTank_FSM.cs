using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SmartTank_ThomasTheTank_FSM : AITank
{
    public Dictionary<GameObject, float> targetTanksFound = new Dictionary<GameObject, float>();
    public Dictionary<GameObject, float> consumablesFound = new Dictionary<GameObject, float>();
    public Dictionary<GameObject, float> basesFound = new Dictionary<GameObject, float>();

    public GameObject targetTankPosition;
    public GameObject consumablePosition;
    public GameObject basePosition;

    float currentTime;
    Rigidbody body;
    int rotation;
    Vector3 lookPos = Vector3.zero;

    public Dictionary<string, bool> stats = new Dictionary<string, bool>();
    public Rules rules = new Rules();
    public bool lowHealth;

    //cant use start or update, use below instead
    public override void AITankStart()
    {
        InitFSM();
        body = GetComponent<Rigidbody>();

        stats.Add("lowHealth", lowHealth);
        stats.Add("targetSpotted", false);
        stats.Add("targetReached", false);
        stats.Add("escapeState", false);
        stats.Add("chaseState", false);
        stats.Add("patrolState", false);
        stats.Add("attackState", false);

        rules.AddRule(new Rule("targetSpotted", "lowHealth", typeof(PatrolState_ThomasTheTank_FSM), Rule.Predicate.nAnd)); // if target hasnt been spotted and we arent on low health, go to patrol state
        rules.AddRule(new Rule("chaseState", "lowHealth", typeof(EscapeState_ThomasTheTank_FSM), Rule.Predicate.And));
    }


    public void CheckTargetReached()
    {
        if(Vector3.Distance(transform.position, targetTankPosition.transform.position) < 1.5f)
        {
            stats["targetReached"] = true;
        }
        else
        {
            stats["targetReached"] = false;
        }
    }

    private void Update()
    {
        stats["lowHealth"] = lowHealth;


    }
    void InitFSM()
    {
        //creates a dictionary of all states 
        Dictionary<Type, BaseState_ThomasTheTank_FSM> states = new Dictionary<Type, BaseState_ThomasTheTank_FSM>();
        //Add all states like this to the dictionary
        states.Add(typeof(PatrolState_ThomasTheTank_FSM), new PatrolState_ThomasTheTank_FSM(this));
        states.Add(typeof(ChaseState_ThomasTheTank_FSM), new ChaseState_ThomasTheTank_FSM(this));

        //Set states
        GetComponent<FiniteStateMachine_ThomasTheTank_FSM>().SetStates(states);
    }

    public bool IsMoving()
    {
        if (body.velocity != Vector3.zero)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public void MoveToPoint(GameObject follow, float speed)
    {
        FollowPathToPoint(follow, speed);
    }

    public void TurretSpin(int rotation)
    {
        //Needs Fixing, feel free to fix it :)
        switch (rotation)
        {
            case 1:
                lookPos = transform.forward + new Vector3(0, 0, 0);
                Debug.Log(lookPos);
                break;
            case 2:
                lookPos = transform.forward + new Vector3(45, 0, 0);
                Debug.Log(lookPos);
                break;
            case 3:
                lookPos = transform.forward + new Vector3(90, 0, 0);
                Debug.Log(lookPos);
                break;
            case 4:
                lookPos = transform.forward + new Vector3(90, 0, 45);
                Debug.Log(lookPos);
                break;
            case 5:
                lookPos = transform.forward + new Vector3(90, 0, 90);
                Debug.Log(lookPos);
                break;
            case 6:
                lookPos = transform.forward + new Vector3(45, 0, 90);
                Debug.Log(lookPos);
                break;
            case 7:
                lookPos = transform.forward + new Vector3(0, 0, 90);
                break;
            case 8:
                lookPos = transform.forward + new Vector3(0, 0, 45);
                Debug.Log(lookPos);
                break;
            default:
                break;
        }
       
        
        FaceTurretToPoint(lookPos);
    }

    public void SearchRandomPoint()
    {
        //searching
        targetTankPosition = null;
        consumablePosition = null;
        basePosition = null;
        FollowPathToRandomPoint(1f);
        currentTime += Time.deltaTime;
        if (currentTime > 10)
        {
            GenerateRandomPoint();
            currentTime = 0;
        }
    }



    public override void AITankUpdate()
    {
        //Get the targets found from the sensor view
        targetTanksFound = GetAllTargetTanksFound;
        consumablesFound = GetAllConsumablesFound;
        basesFound = GetAllBasesFound;

        //if there is a target found
        if (targetTanksFound.Count > 0 && targetTanksFound.First().Key != null)
        {
            targetTankPosition = targetTanksFound.First().Key;
        }


            /*
            //if low health or ammo, go searching
            if (GetHealthLevel < 50 || GetAmmoLevel < 5)
            {
                if (consumablesFound.Count > 0)
                {
                    consumablePosition = consumablesFound.First().Key;
                    FollowPathToPoint(consumablePosition, 1f);
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
                    FollowPathToRandomPoint(1f);
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
                            FireAtPoint(targetTankPosition);
                        }
                        else
                        {
                            FollowPathToPoint(targetTankPosition, 1f);
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
                            FireAtPoint(basePosition);
                        }
                        else
                        {
                            FollowPathToPoint(basePosition, 1f);
                        }
                    }
                }
                else
                {
                    //searching
                    targetTankPosition = null;
                    consumablePosition = null;
                    basePosition = null;
                    FollowPathToRandomPoint(1f);
                    currentTime += Time.deltaTime;
                    if (currentTime > 10)
                    {
                        GenerateRandomPoint();
                        currentTime = 0;
                    }
                }
            }
            */
        }

    public override void AIOnCollisionEnter(Collision collision)
    {
      
    }

}
