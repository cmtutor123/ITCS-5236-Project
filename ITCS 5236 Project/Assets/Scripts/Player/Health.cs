using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DeathManager))]
public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100;

    private float currentHealth;

    private DeathManager deathManager;

    void Start()
    {
        deathManager = GetComponent<DeathManager>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (currentHealth <= 0)
        {
            deathManager.TriggerDeath();
        }
    }
}
