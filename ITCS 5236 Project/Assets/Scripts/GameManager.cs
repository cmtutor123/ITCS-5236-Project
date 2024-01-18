using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Color[] shipColors = new Color[4];

    public float waveProgress = 0;

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

    private bool[] upgradeSelected = new bool[4];

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
            if (currentPlayer != null) currentPlayer.SetPlayerId(i);
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
        Debug.Log("Start Game");
        for (int i = 0; i < 4; i++)
        {
            PlayerManager playerManager = playerManagers[i];
            if (playerManager != null)
            {
                playerManager.InitializeUpgradeLists();
                playerManager.SetColor(shipColors[i]);
            }
        }
        StartCoroutine(StartRound());
    }

    public IEnumerator StartRound()
    {
        Debug.Log("Start Round");
        enemyCap = 5 + Mathf.Clamp(currentWave / 2, 0, 10);
        Debug.Log("Enemy Cap: " + enemyCap);
        enemySpawnDelay = BASE_ENEMY_SPAWN_DELAY - 0.4f * Mathf.Clamp(currentWave, 0, 12);
        Debug.Log("Enemy Spawn Delay: " + enemySpawnDelay);
        playerBaseManager.StartWave(Mathf.Clamp(5 + currentWave, 5, 15));
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
        Debug.Log("Spawn All Players");
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
        Debug.Log("Starting Enemy Wave");
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
        Debug.Log("Player " + (id + 1) + " Exists: " + playerManagers[id] != null);
        return playerManagers[id] != null;
    }

    public void SpawnPlayer(int id)
    {
        Debug.Log("Spawn Player " + (id + 1));
        playerManagers[id].SpawnPlayer();
    }

    public void SpawnEnemies(int wave)
    {
        Debug.Log("Spawning Enemies");
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

        //Debug.Log(newEnemy.tag);
        switch(newEnemy.tag) {
            case "EnemyPlayer":
                //Debug.Log("EnemyPlayer created");
                targetTransform = null;
                break;

            case "EnemyBase":
                //Debug.Log("EnemyBase created");
                targetTransform = playerBase.transform; 
                break;

            case "EnemyDrop":

                //Debug.Log(drops.Count);
                if(drops.Count > 0) {
                    //Debug.Log("EnemyDrop created");
                    //targetTransform = clusterAlgorith.GetDestination().transform;
                }
                else {
                    //targetTransform = null;
                }
                break;

            default:
                //Debug.Log("ERROR: Default created");
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
        else return currentPlayer.GetHealth();
    }
    public float GetBaseHealth(){
        return playerBase.GetComponent<Health>().GetHealth();
    }
    public void RestartGame(){
        UnjoinPlayerManagers();
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
        //Debug.Log("Start Upgrade Select");
        for (int i = 0; i < playerManagers.Length; i++)
        {
            PlayerManager playerManager = playerManagers[i];
            if (playerManager != null)
            {
                upgradeSelected[i] = false;
                List<Upgrade> upgradeSelection = playerManager.GetUpgradeSelection();
                for (int j = 0; j < 3; j++)
                {
                    int k = i * 3 + j;
                    if (j < upgradeSelection.Count)
                    {
                        uiControl.SetAbilityText(k, upgradeSelection[j].upgradeName, upgradeSelection[j].upgradeDescription);
                    }
                    else
                    {
                        uiControl.SetAbilityText(k, "Dud", "No Effect");
                    }
                }
            }
            else
            {
                upgradeSelected[i] = true;
            }
        }
        uiControl.ShowUpgradeSelect();
    }

    public void HideUpgrades(int player)
    {
        PlayerManager playerManager = playerManagers[player];
        if (playerManager != null)
        {
            for (int j = 0; j < 3; j++)
            {
                int k = player * 3 + j;
                uiControl.SetAbilityText(k, "", "");
            }
        }
    }

    public void UpgradeSelected(int player, int upgrade)
    {
        //Debug.Log("Player = " + player + " | Upgrade = " + upgrade);
        if (upgradeSelected[player])
        {
            //Debug.Log("Upgrade Already Selected");
            return;
        }
        if (playerManagers[player] != null)
        {
            playerManagers[player].SelectUpgrade(upgrade);
            upgradeSelected[player] = true;
            HideUpgrades(player);
        }
        if (upgradeSelected[0] && upgradeSelected[1] && upgradeSelected[2] && upgradeSelected[3])
        {
            uiControl.HideUpgradeSelect();
            StartCoroutine(StartRound());
        }
    }

    public void UnjoinPlayerManagers()
    {
        foreach (PlayerManager playerManager in playerManagers)
        {
            if (playerManager != null)
            {
                playerManager.UnjoinPlayer();
            }
        }
    }

    public int GetScore()
    {
        return ((int) (100 * (currentWave + waveProgress))) * 10;
    }
}
