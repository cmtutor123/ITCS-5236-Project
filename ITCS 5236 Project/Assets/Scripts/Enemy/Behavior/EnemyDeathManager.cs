using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyDeathManager : MonoBehaviour
{
    // When enemy health runs out this method starts
    public void TriggerDeath() {
        // destroy enemy
        Destroy(gameObject);
    }
}
