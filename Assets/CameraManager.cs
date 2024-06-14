using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    [Header("Serialized Fields")]
    [SerializeField] CinemachineVirtualCamera _camera;
    [SerializeField] CinemachineImpulseSource _impulseSource;

    // Start is called before the first frame update
    void Start()
    {
        LocalPlayerSpawnController.Instance.OnLocalPlayerSpawn += SetCameraTarget;
    }

    void SetCameraTarget(GameObject target)
    {
        _camera.Follow = target.transform;

        HealthSystem targetHealthSystem = target.GetComponentInChildren<HealthSystem>(); //Tries to subscribe the event to the health system is it exists
        if (targetHealthSystem != null)
        {
            targetHealthSystem.OnPlayerDamaged += _ => CreateScreenShake(.5f);
        }
    }

    public void CreateScreenShake(float force)
    {
        _impulseSource.GenerateImpulseWithForce(force);
    }
}
