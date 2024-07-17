using System.Collections;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using QFSW.QC;
using System;
using Random = UnityEngine.Random;


public class WalkerAI : NetworkBehaviour
{
    [HideInInspector] public AIMovement movementHandler;

    [Header("Properties")]
    [SerializeField] float _detectionRadius = 100;
    [Range(0,360)][SerializeField] float _fieldOfView = 120;
    public enum State
    {
        Roam,
        Chase,
        Search,
        Investigate
    }


    public ObservableVariable<State> CurrentState = new(State.Roam);

    [Header("Behaviour: Roam")]
    [SerializeField] float _minRoamRange = 0;
    [SerializeField] float _maxRoamRange = 10;
    [SerializeField] bool _drawRoamRadius;

    [Header("Behaviour: Chase")]
    public float AttackRange = 5;
    [SerializeField] float _attackDamage = 25;
    [SerializeField] float _attackCooldown = 3f;
    [SerializeField] bool _drawAttackRange;

    [Header("Behaviour: Search")]
    [SerializeField] float _aggroDurationAfterTargetLost = 5;
    [SerializeField] float _delayBeforeContinuingToRoam = 3;

    [Header("Debug")]
    [SerializeField] bool _targetVisible = false;
    [SerializeField] bool _attackOnCooldown = false;
    [SerializeField] Transform _target;
    [SerializeField] AINoiseSensor _noiseSensor;

    public event Action OnAttack;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsServer)
        {
            enabled = false;
            return;
        }

        movementHandler = GetComponent<AIMovement>();
        GetComponentInChildren<DamageSource>().SetDamage(_attackDamage);

        _noiseSensor = GetComponentInChildren<AINoiseSensor>();
        _noiseSensor.OnCloserNoiseDetected += InvestigatePoint;

        StartCoroutine(RoamOnStart());
    }

    IEnumerator RoamOnStart()
    {
        yield return null;
        RoamToRandomPoint(_maxRoamRange);
    }

    private void Start()
    {
        OnAttack += GetComponent<EntityAnimationHandler>().PlayAttackAnimation;
    }

    private void FixedUpdate()
    {
        if (!IsServer) return;

        SearchForPlayers();
        TryToAttack();
    }


    void InvestigatePoint(Vector2 point)
    {
        SwitchState(State.Investigate);

        movementHandler.MoveToPoint(point, true);
        movementHandler.OnReachedPoint += ReachedNoiseSource;
    }

    void ReachedNoiseSource()
    {
        movementHandler.OnReachedPoint -= ReachedNoiseSource;
        StartCoroutine(ContinueRoaming());
    }

    IEnumerator ContinueRoaming()
    {
        float timeToContinue = 2;

        while (timeToContinue > 0)
        {
            yield return null;
            timeToContinue -= Time.deltaTime;

            if (_noiseSensor.AssignedNoisePoint != Vector2.zero)
            {
                yield break;
            }
        }

        SwitchState(State.Roam);
    }


    #region Attacking Algorithm
    //void AttackTarget()
    //{
    //    HealthSystem targetHealthSystem = _target.GetComponent<HealthSystem>();

    //    Debug.Log($"Attacking {targetHealthSystem}");

    //    if (targetHealthSystem == null)
    //    {
    //        Debug.LogWarning($"{gameObject} has attempted to attack a target [{_target}] without a health system!");
    //        return;
    //    }

    //    targetHealthSystem.DamageEntity(_attackDamage);
    //    _attackOnCooldown = true;

    //    StartCoroutine(ExecuteCooldown());
    //}

    private void TryToAttack()
    {
        if (CurrentState.Value == State.Chase)
        {
            float distanceFromTarget = Vector2.Distance(transform.position, _target.position);
            if (distanceFromTarget <= AttackRange && !_attackOnCooldown)
            {
                OnAttack.Invoke();
                StartCoroutine(ExecuteCooldown());
            }
        }
    }

    IEnumerator ExecuteCooldown()
    {
        _attackOnCooldown = true;
        yield return new WaitForSeconds(_attackCooldown);
        _attackOnCooldown = false;
    }
    #endregion

    #region Roaming Algorithm
    void RoamToRandomPoint(float maxDistance)
    {
        Vector2 currentPosition = transform.position;

        bool pointFound = false;
        Vector2 newPoint = Vector2.zero;
        int attempts = 0;

        // Keep sampling points until a reachable one is found or maximum attempts are reached
        while (!pointFound && attempts < 30)
        {
            float pointX = Random.Range(currentPosition.x - maxDistance, currentPosition.x + maxDistance);
            float pointY = Random.Range(currentPosition.y - maxDistance, currentPosition.y + maxDistance);

            newPoint = new Vector2(pointX, pointY);

            if (NavMesh.SamplePosition(newPoint, out NavMeshHit navMeshHit, maxDistance, NavMesh.AllAreas))
            {
                NavMeshPath path = new NavMeshPath();
                newPoint = navMeshHit.position;

                // Check if the sampled point is reachable

                if (NavMesh.CalculatePath(currentPosition, newPoint, NavMesh.AllAreas, path) && path.status == NavMeshPathStatus.PathComplete)
                {
                    //Debug.Log(CalculatePathLength(path));
                    //Debug.Log($"Attemping to go to point: ({navMeshHit.position.x},{navMeshHit.position.y}");
                    pointFound = true;
                }
            }

            attempts++;
        }

        // If a reachable point is found, move the character to it
        if (pointFound)
        {
            movementHandler.MoveToPoint(newPoint, false);
            movementHandler.OnReachedPoint += ReachedRoamPoint;
        }
        else
        {
            // Handle the scenario where no reachable point is found after maximum attempts
            Debug.LogWarning("No reachable point found within the specified range after maximum attempts.");
        }
    }

    void ReachedRoamPoint()
    {
        movementHandler.OnReachedPoint -= ReachedRoamPoint;
        
        if (CurrentState.Value == State.Roam)
        {
            //Debug.Log("Reached point!");
            StartCoroutine(RoamAfterDelay(3));
        }
    }
    #endregion

    IEnumerator RoamAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        RoamToRandomPoint(_maxRoamRange);
    }

    void SearchForPlayers()
    {
        NetworkObject closestVisiblePlayer = GetClosestVisiblePlayer();
        _targetVisible = (closestVisiblePlayer != null);

        if (_targetVisible && CurrentState.Value != State.Chase)
        {
            ChasePlayer(closestVisiblePlayer);
        }
        else if (CurrentState.Value == State.Chase && !_targetVisible)
        {
            StartCoroutine(FollowTargetAfterLosingSight(_aggroDurationAfterTargetLost, _delayBeforeContinuingToRoam));
        }
    }

    IEnumerator FollowTargetAfterLosingSight(float followDuration, float waitDuration)
    {
        SwitchState(State.Search);

        while (followDuration > 0)
        {
            if (_targetVisible)
            {
                //Debug.Log("Found target!");
                SwitchState(State.Chase); ;
                yield break;
            }

            followDuration -= Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(waitDuration);

        //Debug.Log("Lost target");
        SwitchState(State.Roam);
    }

    void ChasePlayer(NetworkObject player)
    {
        movementHandler.OnReachedPoint -= ReachedRoamPoint;
        //Debug.Log($"Chasing player: {player.name}");
        SwitchState(State.Chase);
        movementHandler.FollowTarget(player);
        _target = player.transform;
    }

    #region Detection Algorithms
    NetworkObject GetClosestVisiblePlayer()
    {
        NetworkObject[] playersWithinDetectionRadius = GetPlayersWithinRadius(_detectionRadius);

        if (playersWithinDetectionRadius.Length == 0) return null; //Return if no player is detected
        return playersWithinDetectionRadius.Where(player => player != null && CanSeePlayer(player.transform)).FirstOrDefault();
    }

    bool CanSeePlayer(Transform playerTransform) //Fire a raycast towards a player to check if there is anything inbetween
    {
        Vector2 direction = (playerTransform.position - transform.position).normalized;

        float currentAngle = Vector2.SignedAngle(direction, transform.up);
        float minAngle = -0.5f * _fieldOfView;
        float maxAngle = 0.5f * _fieldOfView;

        if (currentAngle > maxAngle || currentAngle < minAngle) return false;

        RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, direction, _detectionRadius); //Send a raycast to ensure the player is visible

        if (playerTransform == null) return false; //A null check because for some reason stopping the game would produce an error in the next line

        if (raycastHit.transform.root != playerTransform) return false; //Checks if the raycast trasnform's ROOT is the player transform

        return true;
    }

    NetworkObject[] GetPlayersWithinRadius(float maxRadius)
    {
        NetworkObject[] playersWithinRadius = ServerPlayerObjectManager.Instance.PlayerObjects
            .Where(pair => Vector2.Distance(pair.Value.transform.position, transform.position) <= maxRadius)
            .Select(pair => pair.Value)
            .OrderBy(player => Vector2.Distance(player.transform.position, transform.position))
            .ToArray();

        return playersWithinRadius;
    }
    #endregion

    [Command]

    private void OnDrawGizmos()
    {
        // Set the color of the field of view gizmosh
        Gizmos.color = Color.yellow;

        // Draw the field of view cone
        Vector3 forwardDirection = transform.up;
        Vector3 rightDirection = Quaternion.Euler(0, 0, _fieldOfView * 0.5f) * forwardDirection;
        Vector3 leftDirection = Quaternion.Euler(0, 0, -_fieldOfView * 0.5f) * forwardDirection;

        Gizmos.DrawLine(transform.position, transform.position + rightDirection * _detectionRadius);
        Gizmos.DrawLine(transform.position, transform.position + leftDirection * _detectionRadius);

        if (_drawRoamRadius)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _minRoamRange);
            Gizmos.DrawWireSphere(transform.position, _maxRoamRange);
        }

        if (_drawAttackRange)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, AttackRange);
        }
    }

    public void SwitchState(State state)
    {
        CurrentState.Value = state;

        switch (state)
        {
            case State.Roam:
                RoamToRandomPoint(_maxRoamRange);
                break;

            case State.Chase:
                break;

            case State.Search:
                break;

            case State.Investigate:
                break;
        }
    }
}


//float CalculatePathLength(NavMeshPath path)
//{
//    if (path == null) return 1000;

//    var corners = path.corners;
//    var fullDistance = 0f;

//    for (int i = 1; i < corners.Length; i++)
//    {
//        fullDistance += Vector3.Distance(corners[i - 1], corners[i]);
//    }

//    return fullDistance;
//}