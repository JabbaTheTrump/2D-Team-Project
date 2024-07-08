using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

class AINoiseManager : ServerSideNetworkedBehaviour
{
    [HideInInspector] public static AINoiseManager Instance;

    List<NoiseInstance> _currentNoises = new List<NoiseInstance>();

    [SerializeField] float _timeBetweenNoiseQueries = 0.5f;

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

    IEnumerator QueryNoises()
    {
        while (true)
        {
            yield return new WaitForSeconds(_timeBetweenNoiseQueries);

            foreach (NoiseInstance noise in _currentNoises) //Queries every noise instance in the list
            {
                // Reduces the noise's duration and deletes it if it reaches 0
                noise.RemainingDuration -= Time.deltaTime;

                if (0 >= noise.RemainingDuration)
                {
                    _currentNoises.Remove(noise);
                    continue;
                }
                //

                QueryNoise(noise);
            }
        }
    }

    private void Start()
    {
        StartCoroutine(QueryNoises());
    }

    void QueryNoise(NoiseInstance noise)
    {
        Collider2D[] enemyColliders = Physics2D.OverlapCircleAll(noise.Position, noise.Radius);

        if (enemyColliders.Length < 0) return;

        foreach (Collider2D collider in enemyColliders)
        {
            AINoiseSensor sensor = collider.GetComponent<AINoiseSensor>();
            if (sensor == null) break;

            Debug.Log($"{collider.transform.root.name} has detected a noise!");
            sensor.RegisterNoise(noise.Position);
        }
    }

    public void CreateNoiseAtPoint(Vector2 point, float radius)
    {
        Debug.Log("Noise Created!");

        NoiseInstance createdNoise = new NoiseInstance(point, radius); //Creates the noise

        _currentNoises.Add(createdNoise); //Adds it to the list

        QueryNoise(createdNoise); //Queries it for the first time
    }
}

public class NoiseInstance
{
    public Vector2 Position;
    public float Radius;
    public float RemainingDuration;

    public NoiseInstance(Vector2 position, float radius, float duration)
    {
        Radius = radius;
        RemainingDuration = duration;
        Position = position;
    }

    public NoiseInstance(Vector2 position, float radius)
    {
        Radius = radius;
        RemainingDuration = 0.1f;
        Position = position;
    }
}