using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HealthSystem : NetworkBehaviour
{
    public NetworkVariable<float> MaxHealth = new(100);
    public NetworkVariable<float> CurrentHealth = new(100);

    //Events
    public event Action<float> OnPlayerDamaged;
    public event Action<float> OnPlayerHealed;

    public event Action OnDeath;

    void Start()
    {
        if (!IsServer) return;
        CurrentHealth.Value = MaxHealth.Value;
    }

    public void DamageEntity(float damage)
    {
        if (!IsServer)
        {
            Debug.LogWarning($"Client has attempted to damage an entity! {gameObject}");
            return;
        }

        CurrentHealth.Value -= damage;
        OnPlayerDamaged?.Invoke(CurrentHealth.Value);

        if (0 >= CurrentHealth.Value) //Invokes the player death event
        {
            OnDeath?.Invoke();
        }
    }

    public void HealEntity(float health)
    {
        if (!IsServer)
        {
            Debug.LogWarning($"Client has attempted to heal an entity! {gameObject}");
            return;
        }

        CurrentHealth.Value += health;

        if (CurrentHealth.Value > MaxHealth.Value) //Handles a case where the player heals above the max health
        {
            CurrentHealth.Value = MaxHealth.Value;
            return; //Returns in order to avoid invoking the health change event
        }

        OnPlayerHealed?.Invoke(CurrentHealth.Value);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DamageSource")) //Checks if the collider is tagged as a damage source
        {
            DamageSource damageSrc = collision.GetComponent<DamageSource>(); //Gets the damage source belonging to the collider 

            if (damageSrc == null) 
            {
                Debug.LogWarning($"{transform.root.gameObject}'s {gameObject} is tagged as DamageSource, but doesn't contain a component!");
                return;
            }


            DamageEntity(damageSrc.Damage);
            Debug.Log($"{transform.root.gameObject} has been damaged by {collision.transform.root}!");
        }
    }
}
