using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using Unity.Netcode;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

    // Button Holding Interaction
    [SerializeField] Slider _interactionProgressSlider;
    Coroutine _currentInteractionCoroutine;    
    //

    [field: SerializeField] public GameObject HoveredObject { get; private set; }

    //Events
    public event Action<GameObject> OnHoverNewObject;
    public event Action OnHoverStopped;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        _camera = FindObjectOfType<Camera>();
        _interactionProgressSlider = FindObjectsOfType<Slider>(true).Where(slider => slider.tag == "InteractionSlider").FirstOrDefault();

        if (_camera == null)
        {
            Debug.LogWarning("InteractionHandler failed to find camera!");
        }

        if (_interactionProgressSlider == null)
        {
            Debug.LogWarning("InteractionHandler failed to find the interaction slider!");
        }

        _interactionProgressSlider.gameObject.SetActive(false);
        OnHoverNewObject += (_) => OnInteractionCanceled();
        OnHoverStopped += OnInteractionCanceled;
    }

    void Update()
    {
        if (IsOwner)
        {
            GetObjectBeingHovered();
            _interactionProgressSlider.transform.position = Input.mousePosition;
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
            Vector2.Distance(obj.transform.position, transform.position) <= _interactionRange; //Checks if an object is in the interaction layer and within range
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

    public void OnInteractPress()
    {
        if (HoveredObject == null) return;

        IInteractable interactionModule = HoveredObject.GetComponentInChildren<IInteractable>();

        if (0 >= interactionModule.InteractionTime) 
        {
            TryToInteractServerRpc(HoveredObject.GetComponentInChildren<NetworkObject>().NetworkObjectId);
        }
    }

    public void OnInteractHold() 
    {
        if (HoveredObject == null) return;

        IInteractable interactionModule = HoveredObject.GetComponentInChildren<IInteractable>();

        if (interactionModule.InteractionTime > 0)
        {
            _currentInteractionCoroutine = StartCoroutine(StartProgressBarInteraction(interactionModule));
        }
    }

    public void OnInteractionCanceled() //When a progress type interaction is canceled
    {
        if (_currentInteractionCoroutine != null)
        StopCoroutine(_currentInteractionCoroutine);

        Cursor.visible = true;
        _interactionProgressSlider.gameObject.SetActive(false);
    }

    IEnumerator StartProgressBarInteraction(IInteractable interactionModule)
    {
        float timePassedSinceStarted = 0f;
        _interactionProgressSlider.gameObject.SetActive(true);
        Cursor.visible = false;

        _interactionProgressSlider.maxValue = interactionModule.InteractionTime;

        while (interactionModule.InteractionTime >  timePassedSinceStarted) 
        {
            timePassedSinceStarted += Time.deltaTime;

            _interactionProgressSlider.value = timePassedSinceStarted;

            yield return null;
        }

        _interactionProgressSlider.gameObject.SetActive(false);
        Cursor.visible = true;
        TryToInteractServerRpc(HoveredObject.GetComponentInChildren<NetworkObject>().NetworkObjectId);
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
