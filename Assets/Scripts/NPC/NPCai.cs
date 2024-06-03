using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NoOChiGame.Utilts;

public class NPCai : MonoBehaviour
{
    [SerializeField] private State startingState;
    [SerializeField] private float roamingDistanceMax = 7f;
    [SerializeField] private float roamingDistanceMin = 3f;
    [SerializeField] private float roamingTimerMax = 3f;
    [SerializeField] private float detectionRadius = 5f;  // Радиус обнаружения игрока
    [SerializeField] private float chasingSpeed = 3.5f;   // Скорость преследования
    [SerializeField] private float roamingSpeed = 2f;     // Скорость блуждания

    private NavMeshAgent navMeshAgent;
    private State state;
    private float roamingTime;
    private Vector3 roamingPosition;
    private Vector3 startingPosition;

    private GameObject player;  // Игровой объект игрока

    private enum State
    {
        Roaming,
        ChasingPlayer
    }

    private void Start()
    {
        startingPosition = transform.position;
        player = GameObject.FindGameObjectWithTag("Player"); // Находим игрока по тегу
        if (player == null)
        {
            Debug.LogError("Player object with tag 'Player' not found in the scene.");
        }
    }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent is not attached to the GameObject.");
            return;
        }
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        state = startingState;
        navMeshAgent.enabled = true;
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            default:
            case State.Roaming:
                RoamingUpdate();
                break;
            case State.ChasingPlayer:
                ChasingPlayerUpdate();
                break;
        }
    }

    private void RoamingUpdate()
    {
        roamingTime -= Time.deltaTime;
        if (roamingTime < 0)
        {
            Roaming();
            roamingTime = roamingTimerMax;
        }

        // Проверяем расстояние до игрока
        if (player != null && Vector3.Distance(transform.position, player.transform.position) <= detectionRadius && Player.instance.GetPlayerHealth() > 0)
        {
            state = State.ChasingPlayer;
            navMeshAgent.speed = chasingSpeed;
        }
    }
    private void ChasingPlayerUpdate()
    {
        if (player != null && Player.instance.GetPlayerHealth() > 0)
        {
            // Проверяем здоровье игрока
            if (Player.instance.GetPlayerHealth() <= 0)
            {
                state = State.Roaming;
                navMeshAgent.speed = roamingSpeed;
                Roaming();
                return;
            }

            navMeshAgent.SetDestination(player.transform.position);

            // Если игрок вышел за пределы радиуса обнаружения, возвращаемся к блужданию
            if (Vector3.Distance(transform.position, player.transform.position) > detectionRadius)
            {
                state = State.Roaming;
                navMeshAgent.speed = roamingSpeed;
                Roaming();
            }
        }
        else
        {
            state = State.Roaming;
        }
    }

    private void Roaming()
    {
        startingPosition = transform.position;
        roamingPosition = GetRoamingPosition();
        ChangeFacingDirection(startingPosition, roamingPosition);
        if (NavMesh.SamplePosition(roamingPosition, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            navMeshAgent.SetDestination(roamingPosition);
        }
    }

    private Vector3 GetRoamingPosition()
    {
        return startingPosition + Utilts.GetRandomDir() * UnityEngine.Random.Range(roamingDistanceMin, roamingDistanceMax);
    }

    private void ChangeFacingDirection(Vector3 sourcePosition, Vector3 targetPosition)
    {
        if (sourcePosition.x > targetPosition.x)
        {
            transform.rotation = Quaternion.Euler(0, -180, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
