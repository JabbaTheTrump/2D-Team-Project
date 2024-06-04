using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] CinemachineVirtualCamera _camera;

    // Start is called before the first frame update
    void Start()
    {
        PlayerSpawnController.OnLocalPlayerSpawn += SetFollowTarget;
    }

    void SetFollowTarget(GameObject target)
    {
        _camera.Follow = target.transform;
    }
}
