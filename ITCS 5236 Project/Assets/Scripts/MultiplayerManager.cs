using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(PlayerInputManager))]
public class MultiplayerManager : MonoBehaviour
{
    private PlayerInputManager playerInputManager;
    private VisualElement selections;
    private void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
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
        return new Vector2();
        //return new Vector2(uiPosX + uiPosOffsetX * playerId, uiPosY);
    }
}
