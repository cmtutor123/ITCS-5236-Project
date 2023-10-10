using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputManager))]
public class MultiplayerManager : MonoBehaviour
{

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
}
