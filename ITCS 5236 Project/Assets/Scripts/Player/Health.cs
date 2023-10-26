using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100;

    private float currentHealth;

    private UnityEvent deathEvent;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (currentHealth <= 0 && deathEvent != null)
        {
            deathEvent.Invoke();
        }
    }

    public void Damage(float amount)
    {
        currentHealth -= Mathf.Clamp(amount, 0, currentHealth);
    }

    public void Heal(float amount)
    {
        currentHealth += Mathf.Clamp(amount, 0, maxHealth - currentHealth);
    }
}
