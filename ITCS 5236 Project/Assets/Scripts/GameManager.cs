using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const int MAX_PLAYERS = 4;

    private const float PLAYER_SPAWN_DELAY = 0.5f;
    private const float ENEMY_SPAWN_DELAY = 5f;

    private PlayerManager[] playerManagers;
    private int playerCount = 0;
    private List<GameObject> enemies, drops;

    private int currentWave;
    private bool inWave;

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
        playerManager.SetPlayerId(playerCount);
        playerManagers[playerCount] = playerManager;
        playerCount++;
    }

    public void UnregisterPlayer(PlayerManager playerManager)
    {
        if (playerManager == null) return;
        int removedPlayerId = playerManager.GetPlayerId();
        playerManagers[removedPlayerId] = null;
        for (int i = removedPlayerId; i < MAX_PLAYERS - 1; i++)
        {
            PlayerManager currentPlayer = playerManagers[i + 1];
            currentPlayer.SetPlayerId(i);
            playerManagers[i] = currentPlayer;
            playerManagers[i + 1] = null;
        }
        playerCount--;
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

    public List<PlayerManager> GetPlayers()
    {
        List<PlayerManager> playerList = new List<PlayerManager>();
        for (int i = 0; i < playerCount; i++) playerList.Add(playerManagers[i]);
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
        StartCoroutine(StartRound());
    }

    public IEnumerator StartRound()
    {
        yield return InitialPlayerSpawn();
        inWave = true;
        yield return StartEnemyWaves(currentWave);
    }

    public void EndRound()
    {
        inWave = false;
        currentWave++;
    }

    public IEnumerator InitialPlayerSpawn()
    {
        yield return new WaitForSeconds(PLAYER_SPAWN_DELAY);
        if (PlayerExists(0)) SpawnPlayer(0);
        yield return new WaitForSeconds(PLAYER_SPAWN_DELAY);
        if (PlayerExists(1)) SpawnPlayer(1);
        yield return new WaitForSeconds(PLAYER_SPAWN_DELAY);
        if (PlayerExists(2)) SpawnPlayer(2);
        yield return new WaitForSeconds(PLAYER_SPAWN_DELAY);
        if (PlayerExists(3)) SpawnPlayer(3);
        yield return new WaitForSeconds(PLAYER_SPAWN_DELAY);
    }

    public IEnumerator StartEnemyWaves(int wave)
    {
        if (InWave(wave)) SpawnEnemies(wave);
        yield return new WaitForSeconds(ENEMY_SPAWN_DELAY);
        if (InWave(wave)) yield return StartEnemyWaves(wave);
    }

    public bool InWave(int wave)
    {
        return inWave && wave == currentWave;
    }

    public bool PlayerExists(int id)
    {
        return playerManagers[id] != null;
    }

    public void SpawnPlayer(int id)
    {
        playerManagers[id].SpawnPlayer();
    }

    public void SpawnEnemies(int wave)
    {
        Debug.Log("Spawning Enemies");
    }
}
