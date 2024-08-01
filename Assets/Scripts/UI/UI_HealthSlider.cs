using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UI_HealthSlider : MonoBehaviour
{
    [SerializeField] Slider _slider;

    HealthSystem _playerHealth;

    // Start is called before the first frame update
    private void Start()
    {
        LocalPlayerObjectManager.Instance.OnLocalPlayerSpawned += Initialize;
    }

    private void Initialize(GameObject playerObj)
    {
        _playerHealth = playerObj.GetComponentInChildren<HealthSystem>();

        if (_playerHealth == null)
        {
            Debug.LogWarning("Health slider failed to find a health system on player!");
            return;
        }

        _playerHealth.MaxHealth.OnValueChanged += (_, __) => UpdateHealthSlider();
        _playerHealth.CurrentHealth.OnValueChanged += (_, __) => UpdateHealthSlider();

        UpdateHealthSlider();
    }

    void UpdateHealthSlider()
    {
        _slider.maxValue = _playerHealth.MaxHealth.Value;
        _slider.value = _playerHealth.CurrentHealth.Value;
    }
}
