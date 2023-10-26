using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    private GameManager gameManager;

    [SerializeField] private GameObject prefabPlayerShip;

    private bool isAlive = false;

    private GameObject playerShip = null;

    private PlayerController playerShipController;

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

    public void SpawnPlayer()
    {
        playerShip = Instantiate(prefabPlayerShip);
        playerShipController = playerShip.GetComponent<PlayerController>();
        isAlive = true;
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (isAlive) playerShipController.AimOnPerformed(context);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (isAlive) playerShipController.MoveOnPerformed(context);
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (isAlive) playerShipController.ShootOnPerformed(context);
    }

    public void OnAbility(InputAction.CallbackContext context)
    {
        if (isAlive) playerShipController.AbilityOnPerformed(context);
    }

    public void OnTether(InputAction.CallbackContext context)
    {
        if (isAlive) playerShipController.TetherOnPerformed(context);
    }
}
