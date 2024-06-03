using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NoOChiGame.Utilts;
public class Muzhik : MonoBehaviour
{
    [SerializeField] private State startingState;
    [SerializeField] private float roamingDistanceMax = 7f;
    [SerializeField] private float roamingDistanceMin = 3f;
    [SerializeField] private float roamingTimerMax = 3f;

    private NavMeshAgent navMeshAgent;
    private State state;
    private float roamingTime;
    private Vector3 roamingPosition;
    private Vector3 startingPosition;

    private enum State
    {
        Idle,
        Roaming
    }
    private void Start()
    {
        startingPosition = transform.position;
    }
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        state = startingState;
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            default:
            case State.Idle: 
                break;
            case State.Roaming:
                roamingTime -= Time.deltaTime;
                if(roamingTime < 0)
                {
                    Roaming();
                    roamingTime = roamingTimerMax;
                }
                break;
        }
    }
    private void Roaming() 
    {
        startingPosition = transform.position;
        roamingPosition = GetRoamingPosition();
        navMeshAgent.SetDestination(roamingPosition);
    }
    private Vector3 GetRoamingPosition()
    {
        return startingPosition + Utilts.GetRandomDir() * UnityEngine.Random.Range(roamingDistanceMin, roamingDistanceMax);
    }
}
