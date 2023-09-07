using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunBehavior : MonoBehaviour
{
    [SerializeField] private Behavior behavior;

    void Update()
    {
        behavior.RunBehavior();
    }
}
