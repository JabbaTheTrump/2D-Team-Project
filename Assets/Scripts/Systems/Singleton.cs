using Unity.Netcode;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"Detected duplicate singleton script of type {Instance}!");
            Debug.LogWarning($"Roots: {transform.root},  {Instance.transform.root}");
            Destroy(this);
        }
        else
        {
            Instance = this as T;
        }
    }
}

public class NetworkSingleton<T> : NetworkBehaviour where T : NetworkSingleton<T>
{
    public static T Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"Detected duplicate singleton script of type {Instance}!");
            Debug.LogWarning($"Roots: {transform.root},  {Instance.transform.root}");
            Destroy(this);
        }
        else
        {
            Instance = this as T;
        }
    }
}

public class ServerSingleton<T> : ServerSideNetworkedBehaviour where T : ServerSingleton<T>
{
    public static T Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this as T;
        }
    }
}