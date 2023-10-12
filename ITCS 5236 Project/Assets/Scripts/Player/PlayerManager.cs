using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject prefabPlayerShip;
    [SerializeField] private GameObject prefabPlayerSelectUI;

    private GameObject playerSelectUI;

    private MultiplayerManager multiplayerManager;

    private int playerId;

    private bool hasPlayerObject;

    void Start()
    {
        multiplayerManager = GetComponent<MultiplayerManager>();
        playerId = multiplayerManager.GetNextPlayerId();
    }

    void Update()
    {
        
    }

    public bool HasPlayerObject()
    {
        return hasPlayerObject;
    }

    public void LoadPlayerSelectUI()
    {
        playerSelectUI = Instantiate(prefabPlayerSelectUI);
        playerSelectUI.transform.position = multiplayerManager.GetPlayerUIPosition(playerId);
    }
}
