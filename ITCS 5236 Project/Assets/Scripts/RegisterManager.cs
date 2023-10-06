using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterManager : MonoBehaviour
{
    private GameManager gameManager;

    [SerializeField] private bool player, enemy, drop;

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        if (player) gameManager.RegisterPlayer(gameObject, GetComponent<PlayerController>().GetPlayerId());
        else if (enemy) gameManager.RegisterEnemy(gameObject);
        else if (drop) gameManager.RegisterDrop(gameObject);
    }

    private void OnDestroy()
    {
        if (player) gameManager.UnregisterPlayer(GetComponent<PlayerController>().GetPlayerId());
        else if (enemy) gameManager.UnregisterEnemy(gameObject);
        else if (drop) gameManager.UnregisterDrop(gameObject);
    }
}
