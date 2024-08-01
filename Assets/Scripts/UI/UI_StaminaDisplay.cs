using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StaminaDisplay : MonoBehaviour
{
    [SerializeField] Image _staminaImage;

    PlayerMovementHandler _playerMovementHandler;

    // Start is called before the first frame update
    void Start()
    {
        SetImageAlpha(0);
        LocalPlayerObjectManager.Instance.OnLocalPlayerSpawned += Initialize;
    }

    private void Initialize(GameObject player)
    {
        _playerMovementHandler = player.GetComponentInChildren<PlayerMovementHandler>();

        if (_playerMovementHandler == null)
        {
            Debug.LogWarning("Stamina UI display couldn't find a MovementHandler component on the player");
            return;
        }

        _playerMovementHandler.OnStaminaChanged += StaminaChanged;
    }

    void StaminaChanged(float currentStamina)
    {
        if (currentStamina > _playerMovementHandler.StaminaSprintThreshold) //Makes the image invisible if the stamina above the sprint threshold
        {
            SetImageAlpha(0); 
            return;
        }

        float threshold = _playerMovementHandler.StaminaSprintThreshold;
        float newAlpha = (threshold - currentStamina) / threshold; //Sets the opacity according to how little stamina there is left (more stamina = less visible)

        SetImageAlpha(newAlpha);
    }

    void SetImageAlpha(float alpha)
    {
        Color temp = _staminaImage.color;
        temp.a = alpha;

        _staminaImage.color = temp;
    }
}
