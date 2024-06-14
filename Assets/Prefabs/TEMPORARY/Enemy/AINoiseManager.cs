using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

class AINoiseManager : NetworkBehaviour
{
    [HideInInspector] public static AINoiseManager Instance;

    List<NoiseInstance> currentNoises = new List<NoiseInstance>();


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }
    }

    IEnumerator QueryNoises(float timeBetweenQueries)
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenQueries);

            foreach (NoiseInstance noise in currentNoises)
            {

            }
        }
    }

    private void Start()
    {
        if (!IsServer) enabled = false;
    }

    public void CreateNoiseAtPoint(Vector2 point, float distance)
    {
        Debug.Log("Noise Created!");
        Collider2D[] enemyColliders = Physics2D.OverlapCircleAll(point, distance);

        if (enemyColliders.Length < 0) return;

        foreach (Collider2D collider in enemyColliders)
        {
            AINoiseSensor sensor = collider.GetComponent<AINoiseSensor>();
            if (sensor == null) break;

            Debug.Log($"{collider.transform.root.name} has detected a noise!");
            sensor.RegisterNoise(point);
        }
    }
}

public class NoiseInstance
{
    public Vector2 Position;
    public float Distance;
    public bool IsActive = false;

    public NoiseInstance(float distance, bool startActive)
    {
        Distance = distance;
        IsActive = startActive;
    }
}