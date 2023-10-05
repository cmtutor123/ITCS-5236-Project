using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunBehavior : MonoBehaviour
{
    [SerializeField] public Behavior behavior;

    void FixedUpdate()
    {
        behavior.RunBehavior();
    }

    public void SetBehavior(Behavior behavior)
    {
        this.behavior = behavior;
    }
}
