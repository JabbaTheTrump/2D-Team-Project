using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using Unity.Netcode;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionHandler : NetworkBehaviour
{
    [Header("Properties")]
    [SerializeField] float _interactionRange = 20f;
    [SerializeField] float _interactionOverlapSize = 1f;

    [Space]

    [Header("Debug")]
    [SerializeField] bool _displayInteractionRange = false;
    [SerializeField] bool _displayHoverGizmo = false;
    [SerializeField] LayerMask _interactionLayer;
    [SerializeField] Camera _camera;
    [field: SerializeField] public GameObject HoveredObject { get; private set; }

    //Events
    public event Action<GameObject> OnHoverNewObject;
    public event Action OnHoverStopped;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        _camera = FindObjectOfType<Camera>();
        if (_camera == null)
        {
            Debug.Log("InteractionHandler failed to find camera!");
        }
    }

    void Update()
    {
        if (IsOwner)
        {
            GetObjectBeingHovered();
        }
    }

    void GetObjectBeingHovered()
    {
        Vector2 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);

        Collider2D[] hoveredObjects = Physics2D.OverlapCircleAll(mousePos, _interactionOverlapSize).
            Where(col => IsSelectable(col.gameObject)).ToArray();


        if (hoveredObjects.Length > 0 ) 
        {
            foreach (var obj in hoveredObjects)
            {
                GameObject objRoot = obj.transform.root.gameObject;

                if (objRoot != HoveredObject) //Checks if a NEW object is being hovered
                {
                    if (objRoot.GetComponentInChildren<IInteractable>().IsInteractable.Value) //Checks if the object is set to be interactable;
                    {
                        StartedHovering(objRoot.gameObject);
                    }
                }

                //Debug.Log($"Hovering over {HoveredObject}");
            }
        }
        else if (HoveredObject != null) 
        {
            StoppedHovering();
        }    

    }

    bool IsSelectable(GameObject obj)
    {
        return obj.layer == 6 && 
            Vector2.Distance(obj.transform.position, transform.position) <= _interactionRange;
    }

    void StartedHovering(GameObject target)
    {
        HoveredObject = target.transform.root.gameObject;
        OnHoverNewObject?.Invoke(target);
    }

    void StoppedHovering()
    {
        HoveredObject = null;
        OnHoverStopped?.Invoke();
    }

    void OnInteract_Press() //Idea - implement the holding interaction on the client side, and only call the "interact" method on the object when interaction is finished
    {
        if (HoveredObject != null)
        {
            TryToInteractServerRpc(HoveredObject.GetComponentInChildren<NetworkObject>().NetworkObjectId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void TryToInteractServerRpc(ulong objectId) //Sends the interaction requst to the server to further verify that the interaction is valid
    {
        NetworkObject obj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[objectId];

        if (obj == null)
        {
            Debug.LogWarning($"Attempted to interact with a non-spawned object with ID {objectId}");
            return;
        }

        obj.GetComponentInChildren<IInteractable>().Interact(GetComponent<NetworkObject>());
    }

    private void OnDrawGizmos()
    {
        if (_displayInteractionRange)
        {
            Gizmos.DrawWireSphere(transform.position, _interactionRange);
        }

        if ( _displayHoverGizmo && _camera != null)
        {
            Vector2 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);

            if (HoveredObject != null)
            {
                Gizmos.color = Color.green;
            }

            Gizmos.DrawWireSphere(mousePos, _interactionOverlapSize);
        }
    }
}
