using System;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.AI;

public class AIMovement : NetworkBehaviour
{
    [Header("Properties")]
    [SerializeField] float _walkSpeed;
    [SerializeField] float _runSpeed;
    [SerializeField] float rotationSpeed = 5f;
    [SerializeField] float _doorInteractionRange = 2;

    [Header("Debug Info")]
    [SerializeField] Transform _target;
    [SerializeField] Vector2 _pointTarget;
    [SerializeField] bool _showPointTarget = true;

    public NavMeshAgent NavigationAgent { get; private set; }

    public event Action OnReachedPoint;

    public WalkerAI AIHandler;

    public NetworkVariable<bool> IsWalking = new(false);
    public NetworkVariable<bool> IsChasing = new(false);

    private bool _firedReachPointEvent = false;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsServer)
        {
            enabled = false;
            return;
        }

        NavigationAgent = GetComponent<NavMeshAgent>();
        NavigationAgent.updateRotation = false;
        NavigationAgent.updateUpAxis = false;

        
    }

    private void OnTriggerEnter2D(Collider2D collision) //Handles collision with doors
    {
        if (collision.transform.root.TryGetComponent(out Door door))
        {
            //Debug.Log($"{collision.transform.root.name} has triggered the door.");

            Debug.Log("Door being opened!");

            if (door.ToggledOn.Value) return; //If the door is open do nothing

            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, _doorInteractionRange);

            if (hit.collider.GetComponentInParent<Door>() != door) return;

            door.Interact(GetComponent<NetworkObject>());
        }
    }


    void FixedUpdate()
    {
        if (!IsServer) return;

        if (NavigationAgent.remainingDistance <= 0.5 && _firedReachPointEvent == false)
        {
            OnReachedPoint?.Invoke();
            _firedReachPointEvent = true;
            IsWalking.Value = false;
            IsChasing.Value = false;
            //Debug.Log("AI has reached target point");
        }

        if (_target != null)
        {
            NavigationAgent.stoppingDistance = AIHandler.AttackRange;
            _pointTarget = _target.position;
        }
        else
        {
            NavigationAgent.stoppingDistance = 0;
        }

        if (_pointTarget != Vector2.zero)
        {
            MoveToPosition(_pointTarget);
        }

        RotateTowardsTarget();
    }

    void RotateTowardsTarget()
    {
        Quaternion targetRotation = Quaternion.identity;

        Vector2 currentVelocity = NavigationAgent.velocity;
        if (currentVelocity != Vector2.zero)
        {
            targetRotation = Quaternion.LookRotation(Vector3.forward, currentVelocity);
        }
        else if (_target != null)
        {
            Vector2 directionToTarget = (_target.position - transform.position).normalized;
            targetRotation = Quaternion.LookRotation(Vector3.forward, directionToTarget);
        }

        if (targetRotation == Quaternion.identity) return;

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }


    void MoveToPosition(Vector2 pos)
    {
        _firedReachPointEvent = false;
        NavigationAgent.SetDestination(pos);
        if (IsWalking.Value)
        {
            NavigationAgent.speed = _walkSpeed;
        }
        else
        {
            NavigationAgent.speed = _runSpeed;
        }
    }

    public void WalkToPoint(Vector2 point) //Tell the AI to move to a certain point
    {
        _target = null;
        _pointTarget = point;
        IsWalking.Value = true;
        IsChasing.Value = false;

        // Debug.Log($"Moving to new point: {point}");
    }

    public void FollowTarget(NetworkObject target) //Tell the AI to follow a target
    {
        if (target == null) return;

        _target = target.transform;
        _pointTarget = target.transform.position;

        IsWalking.Value = false;
        IsChasing.Value = true;

        //Debug.Log($"Following new target: {target}");
    }


    public void StopFollowingTarget()
    {
        _target = null;
        //Debug.Log("Stopped following target");
    }

    private void OnDrawGizmos()
    {
        if (_showPointTarget)
        {
            Gizmos.DrawWireSphere(_pointTarget, 0.5f);
        }
    }
}