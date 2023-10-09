using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterManager : MonoBehaviour
{
    private GameManager gameManager;
    private PlayerManager playerManager;
    private int playerId;

    [SerializeField] private bool player, enemy, drop;

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        if (enemy) gameManager.RegisterEnemy(gameObject);
        else if (drop) gameManager.RegisterDrop(gameObject);
    }

    public void SetupPlayer(PlayerManager manager, int id)
    {
        playerManager = manager;
        playerId = id;
    }

    private void OnDestroy()
    {
        if (player) gameManager.UnregisterPlayer(playerId);
        else if (enemy) gameManager.UnregisterEnemy(gameObject);
        else if (drop) gameManager.UnregisterDrop(gameObject);
    }
}
