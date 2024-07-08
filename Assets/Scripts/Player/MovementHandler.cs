using System.Linq;
using UnityEngine;

public enum MovementState
{
    Idle,
    Walking,
    Sprinting
}

[System.Serializable]
public class MovementType
{
    public MovementState State;
    public float Velocity;
    public AudioClip MovementStateAudio;
    public string AnimationTriggerName;
}

public class MovementHandler : ServerSideNetworkedBehaviour
{
    public MovementType[] MovementTypes;
    [field: SerializeField] public ObservableVariable<MovementState> CurrentMovementState { get; protected set; } = new(MovementState.Idle);

    public MovementType GetMovementTypeByState(MovementState state)
    {
        MovementType[] entries = MovementTypes.Where(entry => entry.State == state).ToArray();

        if (entries.Length == 0) //Handles a case where there is no entry for the state
        {
            Debug.LogWarning($"{gameObject} has used 'GetMovementTypeByState' on a non-existing entry!");
            return null;
        }
        else if (entries.Length > 1) Debug.LogWarning($"{gameObject} has multiple entries for the same MovementState!"); //Handles a case where there are multiple entries for a state

        return entries[0];
    }
}
