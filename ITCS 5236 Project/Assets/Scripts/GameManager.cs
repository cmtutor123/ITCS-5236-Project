using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const int MAX_PLAYERS = 4;

    private PlayerManager[] playerManagers;
    private List<GameObject> enemies, drops;

    private void Start()
    {
        InitializeRegisters();
    }

    private void InitializeRegisters()
    {
        playerManagers = new PlayerManager[MAX_PLAYERS];
        enemies = new List<GameObject>();
        drops = new List<GameObject>();
    }

    public void RegisterPlayer(PlayerManager playerManager)
    {
        int newPlayerId = GetNextPlayerId();
        playerManager.SetPlayerId(newPlayerId);
        playerManagers[newPlayerId] = playerManager;
    }

    public void UnregisterPlayer(PlayerManager playerManager)
    {
        int removedPlayerId = playerManager.GetPlayerId();
        playerManagers[removedPlayerId] = null;
        for (int i = removedPlayerId; i < MAX_PLAYERS - 1; i++)
        {
            PlayerManager currentPlayer = playerManagers[i + 1];
            currentPlayer.SetPlayerId(i);
            playerManagers[i] = currentPlayer;
            playerManagers[i + 1] = null;
        }
    }

    public int GetNextPlayerId()
    {
        for (int i = 0; i < MAX_PLAYERS; i++)
        {
            if (playerManagers[i] == null)
            {
                return i;
            }
        }
        return -1;
    }

    public void RegisterEnemy(GameObject enemyObject)
    {
        enemies.Add(enemyObject);
    }

    public void UnregisterEnemy(GameObject enemyObject)
    {
        enemies.Remove(enemyObject);
    }

    public void RegisterDrop(GameObject dropObject)
    {
        enemies.Add(dropObject);
    }

    public void UnregisterDrop(GameObject dropObject)
    {
        enemies.Remove(dropObject);
    }

    public List<GameObject> GetPlayers()
    {
        List<GameObject> playerList = new List<GameObject>();
        //foreach (GameObject player in players) if (player != null) playerList.Add(player);
        return playerList;
    }

    public List<GameObject> GetEnemies()
    {
        return enemies;
    }

    public List<GameObject> GetDrops()
    {
        return drops;
    }

    public void StartGame()
    {

    }

    public void StartRound()
    {

    }

    public void EndRound()
    {

    }
}
