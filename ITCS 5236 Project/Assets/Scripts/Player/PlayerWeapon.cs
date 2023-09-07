using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Weapon", menuName = "Player/Weapon")]
public class PlayerWeapon : ScriptableObject
{
    [Header("Information")]
    [SerializeField] private string weaponName;
    [SerializeField] private string weaponDescription;

    public string GetName()
    {
        return weaponName;
    }

    public string GetDescription()
    {
        return weaponDescription;
    }
}
