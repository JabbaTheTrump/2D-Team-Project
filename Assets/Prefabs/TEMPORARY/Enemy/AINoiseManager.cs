using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

class AINoiseManager : NetworkSingleton<AINoiseManager>
{
    [SerializeField] List<NoiseInstance> _currentNoises = new List<NoiseInstance>();

    [SerializeField] float _timeBetweenNoiseQueries = 0.5f;

    [SerializeField] bool _displayNoiseRadius = true;


    private void Update()
    {
        List<NoiseInstance> tempNoiseList = new List<NoiseInstance>(_currentNoises);

        foreach (NoiseInstance noise in tempNoiseList) //Handles the noises' timers
        {
            noise.RemainingDuration -= Time.deltaTime;

            if (0 >= noise.RemainingDuration)
            {
                _currentNoises.Remove(noise);
            }
        }
    }

    IEnumerator QueryNoises()
    {
        while (true)
        {
            yield return new WaitForSeconds(_timeBetweenNoiseQueries);

            foreach (NoiseInstance noise in _currentNoises) //Queries every noise instance in the list
            {
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
            AINoiseSensor sensor = collider.GetComponentInChildren<AINoiseSensor>();
            if (sensor == null) break;

            Debug.Log($"{collider.transform.root.name} has detected a noise!");
            sensor.RegisterNoise(noise.Position);
        }
    }

    public void CreateNoiseAtPoint(Vector2 point, float radius)
    {
        CreateNoiseAtPoint(point, radius, 0.1f);
    }

    public void CreateNoiseAtPoint(Vector2 point, float radius, float duration)
    {
        NoiseInstance createdNoise = new NoiseInstance(point, radius, duration); //Creates the noise

        _currentNoises.Add(createdNoise); //Adds it to the list

        QueryNoise(createdNoise); //Queries it for the first time
    }

    private void OnDrawGizmos()
    {
        if (_displayNoiseRadius)
        {
            foreach (NoiseInstance noise in _currentNoises)
            {
                Gizmos.DrawWireSphere(noise.Position, noise.Radius);
            }
        }
    }
}


[System.Serializable]
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
}