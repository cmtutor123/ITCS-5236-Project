using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathManager : MonoBehaviour
{
    public void TriggerDeath()
    {
        Destroy(gameObject);
    }
}
