using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorSpriteManager : Singleton<CursorSpriteManager>
{
    [SerializeField] Texture2D _baseTexture;
    [SerializeField] Texture2D _interactTexture;

    // Start is called before the first frame update
    void Start()
    {
        LocalPlayerSpawnController.Instance.OnLocalPlayerSpawn += LocalPlayerSpawned;
    }

    void LocalPlayerSpawned(GameObject playerObject)
    {
        InteractionHandler interactionHandler = playerObject.GetComponentInChildren<InteractionHandler>();

        if (interactionHandler == null)
        {
            Debug.LogWarning("Local Player detected as spawned while not having an interaction handler");
            return;
        }

        interactionHandler.OnHoverNewObject += (_) => StartedHoveringObject();
        interactionHandler.OnHoverStopped += StoppedHoveringObject;

        Cursor.SetCursor(_baseTexture, Vector2.zero, CursorMode.Auto);
    }

    void StartedHoveringObject()
    {
        Cursor.SetCursor(_interactTexture, Vector2.zero, CursorMode.Auto);
    }

    void StoppedHoveringObject()
    {
        Cursor.SetCursor(_baseTexture, Vector2.zero, CursorMode.Auto);
    }
}

