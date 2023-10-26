using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerManager : MonoBehaviour
{
    private GameManager gameManager;

    [SerializeField] private GameObject prefabPlayerShip;

    private int playerId;

    private bool hasPlayerObject;

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        gameManager.RegisterPlayer(this);
    }

    void Update()
    {
        
    }

    public bool HasPlayerObject()
    {
        return hasPlayerObject;
    }

    public void SetPlayerId(int id)
    {
        playerId = id;
    }

    public int GetPlayerId()
    {
        return playerId;
    }
}
