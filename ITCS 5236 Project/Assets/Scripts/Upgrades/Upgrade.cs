using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Upgrade")]
public class Upgrade : ScriptableObject
{
    public string upgradeName = "";
    public string upgradeDescription = "";
    public List<string> stats = new List<string>();
    public List<float> modifiers = new List<float>();
    public List<Upgrade> requirements = new List<Upgrade>();
}
