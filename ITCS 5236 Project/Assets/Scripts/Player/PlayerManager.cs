using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject playerObject;

    private int playerId;

    private bool hasPlayerObject;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public bool HasPlayerObject()
    {
        return hasPlayerObject;
    }    
}
