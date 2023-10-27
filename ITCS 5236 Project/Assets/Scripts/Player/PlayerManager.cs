using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    private GameManager gameManager;

    [SerializeField] private GameObject prefabPlayerShip;

    private GameObject playerShip = null;

    private PlayerController playerShipController;

    private int playerId;

    private bool hasPlayerObject;

    private bool isReady = false;
    private bool onPlayerSelect = true;

    private UIControl uiControl;

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        gameManager.RegisterPlayer(this);
        uiControl = GameObject.FindGameObjectWithTag("PlayerUI").GetComponent<UIControl>();
    }

    void Update()
    {
        
    }

    public GameObject GetPlayerShip()
    {
        return playerShip;
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
        hasPlayerObject = true;
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        Debug.Log("Aim Button Pressed");
        if (hasPlayerObject) playerShipController.AimOnPerformed(context);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Move Button Pressed");
            if (hasPlayerObject) playerShipController.MoveOnPerformed(context);
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        Debug.Log("Shoot Button Pressed");
        if (hasPlayerObject) playerShipController.ShootOnPerformed(context);
    }

    public void OnAbility(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Ability Button Pressed");
            if (hasPlayerObject) playerShipController.AbilityOnPerformed(context);
        }
    }

    public void OnTether(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Tether Button Pressed");
            if (hasPlayerObject) playerShipController.TetherOnPerformed(context);
        }
    }

    public void OnButtonSelect(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Select Button Pressed");
            if (onPlayerSelect && !isReady) ReadyPlayer();
        }
    }

    public void OnButtonBack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Back Button Pressed");
            if (onPlayerSelect && !isReady) ChangeShip();
        }
    }

    public void OnButtonChange(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Change Button Pressed");
            if (onPlayerSelect && isReady) UnreadyPlayer();
            else if (onPlayerSelect && !isReady) UnjoinPlayer();
        }
    }

    public void UnjoinPlayer()
    {
        gameManager.UnregisterPlayer(this);
        uiControl.unjoinPlayer();
        Destroy(gameObject);
    }

    public void UnreadyPlayer()
    {
        isReady = false;
        uiControl.unreadyPlayer();
    }

    public void ReadyPlayer()
    {
        isReady = true;
        uiControl.readyPlayer();
    }

    public void ChangeShip()
    {

    }
}
