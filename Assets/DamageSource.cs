using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class DamageSource : ServerSideNetworkedBehaviour
{
    //[field:SerializeField] public bool CanDamagePlayers { get; private set; } = true;
    //[field: SerializeField] public bool CanDamageEntities { get; private set; } = true;

    [HideInInspector] public float Damage { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        if (!CompareTag("DamageSource")) //Checks if the gameobject has the correct tag
        {
            Debug.Log($"{transform.root.gameObject}'s DamageSource doesn't have an appropriate tag! Assigning now.");
            gameObject.tag = "DamageSource";
        }
    }

    public void SetDamage(float newDamage)
    {
        Damage = newDamage;
    }
}
