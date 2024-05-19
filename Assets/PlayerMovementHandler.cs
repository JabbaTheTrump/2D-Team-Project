using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementHandler : MonoBehaviour
{
    [HideInInspector] public Vector2 moveDir;

    [Header("Parameters")]
    public float walkSpeed;


    [Header("Serialized References")]
    [SerializeField] Rigidbody2D rb2d;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void FixedUpdate()
    {
        rb2d.velocity = moveDir * walkSpeed * Time.deltaTime;
    }
}
