using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Class", menuName = "Player/Class")]
public class PlayerClass : ScriptableObject
{
    [Header("Information")]
    [SerializeField] private string spaceshipName;
    [SerializeField] private string spaceshipDescription;

    [Header("Movement")]
    [SerializeField] private float thrustPower;

    [Header("Tether")]
    [SerializeField] private float tetherDistance;
    [SerializeField] private int maxTetherCount;

    [Header("Weapon")]
    [SerializeField] private PlayerWeapon playerWeapon;

    [Header("Ability")]
    [SerializeField] private PlayerAbility playerAbility;

    [Header("Sprite")]
    [SerializeField] private Sprite spaceshipSprite;

    public string GetName()
    {
        return spaceshipName;
    }

    public string GetDescription()
    {
        return spaceshipDescription;
    }

    public float GetTrustPower()
    {
        return thrustPower;
    }

    public float GetTetherDistance()
    {
        return tetherDistance;
    }

    public int GetMaxTetherCount()
    {
        return maxTetherCount;
    }

    public PlayerWeapon GetPlayerWeapon()
    {
        return playerWeapon;
    }

    public PlayerAbility GetPlayerAbility()
    {
        return playerAbility;
    }

    public Sprite GetSpaceshipSprite()
    {
        return spaceshipSprite;
    }
}
