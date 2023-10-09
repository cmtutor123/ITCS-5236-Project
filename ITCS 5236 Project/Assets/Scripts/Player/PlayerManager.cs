using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject playerObject;

    void Start()
    {
        Instantiate(playerObject);
    }

    void Update()
    {
        
    }
}
