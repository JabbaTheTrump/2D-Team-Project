using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerRotationHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RotateTowardsPoint(cursorPosition);

    }

    void RotateTowardsPoint(Vector2 point)
    {
        transform.up = (Vector3)(point - (Vector2)transform.position);
    }
}
