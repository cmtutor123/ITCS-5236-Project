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

    private float respawnTime = 3f;

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
        playerShipController.playerManager = this;
        hasPlayerObject = true;
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        //Debug.Log("Aim Button Pressed");
        if (hasPlayerObject) playerShipController.AimOnPerformed(context);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (false && context.performed)
        {
            //Debug.Log("Move Button Pressed");
            if (hasPlayerObject) playerShipController.MoveOnPerformed(context);
        }
        else if (context.started)
        {
            //Debug.Log("Move Button Started");
            if (hasPlayerObject) playerShipController.MoveOnStarted(context);
        }
        else if (context.canceled)
        {
            //Debug.Log("Move Button Canceled");
            if (hasPlayerObject) playerShipController.MoveOnCanceled(context);
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        Debug.Log("Shoot Button Pressed");
        if(context.performed){
            if (hasPlayerObject) playerShipController.ShootOnPerformed(context);
        }
        if(context.canceled){
            if (hasPlayerObject) playerShipController.ShootOnCanceled(context);
        }
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
        if(context.canceled){
            if (hasPlayerObject) playerShipController.TetherOnCanceled(context);
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
            if (onPlayerSelect && isReady) UnreadyPlayer();
            else if (onPlayerSelect && !isReady) UnjoinPlayer();
        }
    }

    public void OnButtonChange(InputAction.CallbackContext context)
    {
		if (context.performed)
        {
            //Debug.Log("Change Button Pressed");
            if (onPlayerSelect && !isReady) ChangeShip(context.ReadValue<Vector2>().x);
        }
    }

    public void UnjoinPlayer()
    {
        uiControl.unjoinPlayer(playerId);
        gameManager.UnregisterPlayer(this);
        Destroy(gameObject);
    }

    public void UnreadyPlayer()
    {
        isReady = false;
        uiControl.unreadyPlayer(playerId);
    }

    public void ReadyPlayer()
    {
        isReady = true;
        uiControl.readyPlayer(playerId);
    }

    public void ChangeShip(float direction)
    {
        uiControl.PlayerChangeShips(playerId, direction);
    }

    public void Death()
    {
        gameManager.UnregisterPlayer(this);
        Destroy(gameObject);
    }

    public void RespawnPlayer()
    {
        StartCoroutine(RespawnShip());
    }

    public IEnumerator RespawnShip()
    {
        yield return new WaitForSeconds(respawnTime);
        if (GameManager.inRound)
        {
            SpawnPlayer();
        }
    }
}
