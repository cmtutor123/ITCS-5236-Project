using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100;

    private float currentHealth = 100;
    private float regen = 0;
    private float damageMultiplier = 1;

    [SerializeField] private UnityEvent deathEvent;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void FixedUpdate()
    {
        if (currentHealth <= 0 && deathEvent != null)
        {
            deathEvent.Invoke();
        }
        else if (currentHealth > 0)
        {
            Heal(regen * Time.deltaTime);
        }
    }

    public void Damage(float amount)
    {
        currentHealth -= Mathf.Clamp(amount * damageMultiplier, 0, currentHealth);
    }

    public void Heal(float amount)
    {
        currentHealth += Mathf.Clamp(amount, 0, maxHealth - currentHealth);
    }
    public float GetHealth()
    {
        return currentHealth;
    }

    public void SetMaxHealth(float amount)
    {
        maxHealth = amount;
        currentHealth = maxHealth;
        Debug.Log("Health: " + maxHealth);
    }

    public void SetRegen(float amount)
    {
        regen = amount;
        Debug.Log("Regen: " + regen);
    }

    public void SetDamageMultiplier(float amount)
    {
        damageMultiplier = amount;
        Debug.Log("Damage Multiplier: " + amount);
    }
}
