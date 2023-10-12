using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputManager))]
public class MultiplayerManager : MonoBehaviour
{
    private const float uiPosY = 0f;
    private const float uiPosX = 0f;
    private const float uiPosOffsetX = 0f;

    private PlayerInputManager playerInputManager;

    private void Start()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        EnablePlayerJoin();
    }

    public void EnablePlayerJoin()
    {
        playerInputManager.EnableJoining();
    }

    public void DisablePlayerJoin()
    {
        playerInputManager.DisableJoining();
    }

    public int GetNextPlayerId()
    {
        return playerInputManager.playerCount - 1;
    }

    public Vector2 GetPlayerUIPosition(int playerId)
    {
        return new Vector2(uiPosX + uiPosOffsetX * playerId, uiPosY);
    }
}
