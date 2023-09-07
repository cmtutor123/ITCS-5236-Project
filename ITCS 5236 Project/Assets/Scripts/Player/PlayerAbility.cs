using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Ability", menuName = "Player/Ability")]
public class PlayerAbility : ScriptableObject
{
    [Header("Information")]
    [SerializeField] private string abilityName;
    [SerializeField] private string abilityDescription;

    public string GetName()
    {
        return abilityName;
    }

    public string GetDescription()
    {
        return abilityDescription;
    }
}
