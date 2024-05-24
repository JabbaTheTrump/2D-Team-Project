using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MovementHandler : NetworkBehaviour
{
    [HideInInspector] public Vector2 moveDir;

    [Header("Parameters")]
    public float walkSpeed;


    [Header("Serialized References")]
    [SerializeField] Rigidbody2D rb2d;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        enabled = IsOwner; 
    }

    private void FixedUpdate()
    {
        rb2d.velocity = moveDir * walkSpeed * Time.deltaTime;
    }
}
