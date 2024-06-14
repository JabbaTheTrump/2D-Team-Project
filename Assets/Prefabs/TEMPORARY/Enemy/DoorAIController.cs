using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class DoorAIController : NetworkBehaviour
{
    //[SerializeField] private Interactable_Door _door;

    private void Start()
    {
        if (!IsServer)
        {
            enabled = false;
            return;
        }
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    //Debug.Log($"{collision.transform.root.name} has triggered the door.");

    //    if (collision.transform.root.TryGetComponent(out NavMeshAgent _))
    //    {
    //        Debug.Log("Door being opened!");

    //        if (!_door.IsOpen.Value && !_door.IsLocked.Value)
    //        {
    //            _door.IsOpen.Value = true;
    //        }
    //    }
    //}
}
