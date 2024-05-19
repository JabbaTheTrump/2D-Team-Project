using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionHandler : MonoBehaviour
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
    [field: SerializeField] public IInteractable HoveredObject { get; private set; }

    private void Start()
    {
        if (_camera == null )
        {
            _camera = FindObjectOfType<Camera>();
            if (_camera = null)
            {
                Debug.Log("InteractionHandler failed to find camera!");
            }
        }
    }

    void Update()
    {
        GetObjectBeingHovered();
    }

    void GetObjectBeingHovered()
    {
        Vector2 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);

        Collider2D[] hoveredObjects = Physics2D.OverlapCircleAll(mousePos, _interactionOverlapSize). // Get hovered objects
            Where(collider => collider.gameObject.layer == 6). // That are in the interaction layer
            Where(collider => Vector2.Distance(mousePos, transform.position) <= _interactionRange). // And within interaction range
            ToArray();  

        if (hoveredObjects.Length > 0 ) 
        {
            StartedHovering(hoveredObjects[0].gameObject);
            //Debug.Log($"Hovering over {HoveredObject}");
        }
        else
        {
            StoppedHovering();
        }    

    }

    void StartedHovering(GameObject target)
    {
        HoveredObject = target.GetComponent<IInteractable>();
    }

    void StoppedHovering()
    {
        HoveredObject = null;
    }

    void OnInteract()
    {
        HoveredObject?.Interact();
    }


    private void OnDrawGizmos()
    {
        if (_displayInteractionRange)
        {
            Gizmos.DrawWireSphere(transform.position, _interactionRange);
        }

        if ( _displayHoverGizmo)
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
