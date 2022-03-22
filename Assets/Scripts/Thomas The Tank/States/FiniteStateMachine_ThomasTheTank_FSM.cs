using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine_ThomasTheTank_FSM : MonoBehaviour
{
    public enum State
    {
        Patrol, // Tank is searching around for anything it sees
        Chasing, // Enemy Tank found, chasing it down to close enough shooting range
        AttackingTank, // Enemy Tank is close enough, begin attacking the Tank
        AttackingBase, // Attack the Enemyy Base
        Retreat, //Tank is low on specific item / Caugth attacking the Enemy Base
        LowStat // Tank is low on a specific stat, cycle through found items for the closest needed item
    }

    public State currentState;

    // Needed Scripts ------------------------------------------------------------------------
    private PatrolState_ThomasTheTank_FSM patrolStateFSM;
    // ---------------------------------------------------------------------------------------
    private void Start()
    {
       patrolStateFSM = gameObject.GetComponent<PatrolState_ThomasTheTank_FSM>();
    }
    private void Update()
    {
        switch(currentState)
        {
            case State.Patrol:
                Debug.Log("Patrolling");

                break;

            case State.Chasing:
                Debug.Log("Chasing");

                break;


            case State.AttackingTank:
                Debug.Log("Attacking Enemy Tank");

                break;

            case State.AttackingBase:
                Debug.Log("Attacking Enemy Base");

                break;

            case State.Retreat:
                Debug.Log("Retreating");

                break;

            case State.LowStat:
                Debug.Log("Looking for item needed");

                break;
        }
    }
}
