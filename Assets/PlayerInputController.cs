using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField] MovementHandler movementHandler;



    void OnMove(InputValue value)
    {
        movementHandler.moveDir = value.Get<Vector2>();
    }
}
