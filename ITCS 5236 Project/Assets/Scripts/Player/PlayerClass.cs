using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Class", menuName = "Player/Class")]
public class PlayerClass : ScriptableObject
{
    [Header("Information")]
    [SerializeField] public string spaceshipName;
    [SerializeField] public string spaceshipDescription;

    [Header("Ship Stats")]
    [SerializeField] public float thrustPower;
    [SerializeField] public float maxVelocity;
    [SerializeField] public float knockbackResistance;
    [SerializeField] public float damageResistance;

    [Header("Tether Stats")]
    [SerializeField] public float tetherDistance;
    [SerializeField] public int maxTetherCount;
    [SerializeField] public float tetherEfficiency;

    [Header("Weapon Stats")]
    [SerializeField] public PlayerWeapon playerWeapon;
    [SerializeField] public float fireRate;

    [Header("Ability")]
    [SerializeField] public PlayerAbility playerAbility;

    [Header("Sprite")]
    [SerializeField] public Sprite spaceshipSprite;

    public List<(string, float)> GetStats()
    {
        List<(string, float)> stats = new List<(string, float)>();
        return stats;
    }
}
