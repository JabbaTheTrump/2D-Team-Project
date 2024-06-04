using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MapSettingsController : MonoBehaviour
{
    [SerializeField] float _darknessIntensity = 1.0f;
    [SerializeField] float _globalLightIntensity = 0f;

    [SerializeField] SpriteRenderer _darknessSpriteRenderer;
    [SerializeField] Light2D _globalLight;

    private void Start()
    {
        Color clr = _darknessSpriteRenderer.color;
        clr.a = _darknessIntensity * 255;

        _darknessSpriteRenderer.color = clr;

        _globalLight.intensity = _globalLightIntensity;
    }
}