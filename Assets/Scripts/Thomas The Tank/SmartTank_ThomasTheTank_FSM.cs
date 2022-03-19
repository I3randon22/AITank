using System.Collections;
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

    float t;
    Rigidbody rb;

    //cant use start or update, use below instead
    public override void AITankStart()
    {
        InitFSM();
        rb = GetComponent<Rigidbody>();
    }

    void InitFSM()
    {
        //put states and things
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
                FollowPathToPoint(consumablePosition, 1f);
                t += Time.deltaTime;
                if (t > 10)
                {
                    GenerateRandomPoint();
                    t = 0;
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
                    float distance = Vector3.Distance(transform.position, targetTankPosition.transform.position);
                    float time = distance / 60;
                    //Debug.Log(rb.velocity);
                    Vector3 enemyPredictedPos = (targetTankPosition.GetComponent<Rigidbody>().velocity * time) + targetTankPosition.transform.position;
                    GameObject enemyPredictedPosOBJ = new GameObject();

                    enemyPredictedPosOBJ.transform.position = enemyPredictedPos;

                    //get closer to target, and fire
                    if (distance < 60f)
                    {
                        FireAtPoint(enemyPredictedPosOBJ);
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
                t += Time.deltaTime;
                if (t > 10)
                {
                    GenerateRandomPoint();
                    t = 0;
                }
            }
        }
    }

    public override void AIOnCollisionEnter(Collision collision)
    {
        throw new System.NotImplementedException();
    }

}
