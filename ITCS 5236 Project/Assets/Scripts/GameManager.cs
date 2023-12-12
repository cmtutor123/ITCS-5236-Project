using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private const int MAX_PLAYERS = 4;

    private const float PLAYER_SPAWN_DELAY = 0.5f;
    private const float BASE_ENEMY_SPAWN_DELAY = 5f;

    public Transform targetTransform;

    public static float BOUNDRY_X_MIN = -15, BOUNDRY_X_MAX = 15, BOUNDRY_Y_MIN = -8.75f, BOUNDRY_Y_MAX = 8.75f;

    private static PlayerManager[] playerManagers;
    private static int playerCount = 0;
    private List<GameObject> enemies, drops;

    private int currentWave;
    private UIControl uiControl;
    public static bool inRound;

    [SerializeField] private GameObject prefabPlayerBase;
    [SerializeField] private List<GameObject> prefabEnemies;
    //private Cluster clusterAlgorith;

    private GameObject playerBase;
    private PlayerBaseManager playerBaseManager;

    private float enemySpawnDelay;
    private float enemyCap;

    private void Start()
    {
        InitializeRegisters();
        uiControl = GameObject.FindGameObjectWithTag("PlayerUI").GetComponent<UIControl>();

        // clustering algorithm for drop enemys.
        //clusterAlgorith = GetComponent<Cluster>();
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

        if(playerCount == 0) {
            EndGame();
        }
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
        drops.Add(dropObject);
    }

    public void UnregisterDrop(GameObject dropObject)
    {
        drops.Remove(dropObject);
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
        SpawnPlayerBase();
        uiControl.gameStarted = true;
        StartCoroutine(StartRound());
    }

    public IEnumerator StartRound()
    {
        enemyCap = 5 + Mathf.Clamp(currentWave / 2, 0, 10);
        enemySpawnDelay = BASE_ENEMY_SPAWN_DELAY - 0.2f * Mathf.Clamp(currentWave, 0, 20);
        playerBaseManager.StartWave(Mathf.Clamp(20 + 2 * currentWave, 20, 100));
        yield return InitialPlayerSpawn();
        inRound = true;
        yield return StartEnemyWaves(currentWave);
    }

    public void EndRound()
    {
        inRound = false;
        currentWave++;
        DestroyPlayers();
        DestroyEnemies();
        DestroyDrops();
        UpgradeSelect();
    }

    public void EndGame(){
        uiControl.EndGame();
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

    public IEnumerator StartEnemyWaves(int round)
    {
        if (InRound(round)) SpawnEnemies(round);
        Debug.Log("Enemy Count: " + enemies.Count);
        yield return new WaitForSeconds(enemySpawnDelay);
        if (InRound(round) && enemies.Count < enemyCap) yield return StartEnemyWaves(round);
    }

    public bool InRound(int round)
    {
        return inRound && round == currentWave;
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
        if (prefabEnemies.Count == 0) return;
        Vector2 spawnLocation;
        if (Random.Range(0, 2) == 0)
        {
            float xPos = Random.Range(BOUNDRY_X_MIN, BOUNDRY_X_MAX);
            if (Random.Range(0, 2) == 0)
            {
                spawnLocation = new Vector2(xPos, BOUNDRY_Y_MIN);
            }
            else
            {
                spawnLocation = new Vector2(xPos, BOUNDRY_Y_MAX);
            }
        }
        else
        {
            float yPos = Random.Range(BOUNDRY_Y_MIN, BOUNDRY_Y_MAX);
            if (Random.Range(0, 2) == 0)
            {
                spawnLocation = new Vector2(BOUNDRY_X_MIN, yPos);
            }
            else
            {
                spawnLocation = new Vector2(BOUNDRY_X_MAX, yPos);
            }
        }

        GameObject newEnemy;
        // if there are drops then include drop enemy to the random enemy selection
        if(drops.Count <= 0) {
            newEnemy = Instantiate(prefabEnemies[Random.Range(0, 1)], spawnLocation, Quaternion.identity);
        } else {
            newEnemy = Instantiate(prefabEnemies[Random.Range(0, prefabEnemies.Count)], spawnLocation, Quaternion.identity);
        }

        // newEnemy = Instantiate(prefabEnemies[Random.Range(0, prefabEnemies.Count)], spawnLocation, Quaternion.identity);

        Debug.Log(newEnemy.tag);
        switch(newEnemy.tag) {
            case "EnemyPlayer":
                Debug.Log("EnemyPlayer created");
                targetTransform = null;
                break;

            case "EnemyBase":
                Debug.Log("EnemyBase created");
                targetTransform = playerBase.transform; 
                break;

            case "EnemyDrop":

                Debug.Log(drops.Count);
                if(drops.Count > 0) {
                    Debug.Log("EnemyDrop created");
                    //targetTransform = clusterAlgorith.GetDestination().transform;
                }
                else {
                    //targetTransform = null;
                }
                break;

            default:
                Debug.Log("ERROR: Default created");
                targetTransform = null;
                break;
        }
    }

    public void SpawnPlayerBase()
    {
        playerBase = Instantiate(prefabPlayerBase, Vector2.zero, Quaternion.identity);
        playerBaseManager = playerBase.GetComponent<PlayerBaseManager>();
    }


    // Return a random player ship
    public GameObject GetPlayerTarget()
    {
        List<GameObject> alivePlayers = new List<GameObject>();
        for (int i = 0; i < playerCount; i++)
        {
            if (playerManagers[i].HasPlayerObject())
            {
                alivePlayers.Add(playerManagers[i].GetPlayerShip());
            }
        }
        if (alivePlayers.Count == 0) return null;
        return alivePlayers[Random.Range(0, alivePlayers.Count)];
    }
    
    
    public Transform GetDropTarget(){
        if(drops.Count > 0) {
            return drops[Random.Range(0, drops.Count)].transform;
        } else {
            return null;
        }
    }

    public float GetPlayerHealth(int playerIndex)
    {
        PlayerManager currentPlayer = playerManagers[playerIndex];
        if (currentPlayer == null) return 0;
        else if (!currentPlayer.HasPlayerObject()) return 0;
        else return currentPlayer.GetPlayerShip().GetComponent<Health>().GetHealth();
    }
    public float GetBaseHealth(){
        return playerBase.GetComponent<Health>().GetHealth();
    }
    public void RestartGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void DestroyPlayers()
    {
        foreach (PlayerManager playerManager in playerManagers)
        {
            if (playerManager != null)
            {
                playerManager.DestroyPlayer();
            }
        }
    }

    public void DestroyEnemies()
    {
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
    }

    public void DestroyDrops()
    {
        foreach (GameObject drop in drops)
        {
            Destroy(drop);
        }
    }

    public void UpgradeSelect()
    {
        Debug.Log("Start Upgrade Select");
    }
}
