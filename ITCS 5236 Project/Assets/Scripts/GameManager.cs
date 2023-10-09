using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const int MAX_PLAYERS = 4;

    private GameObject[] players;
    private List<GameObject> enemies, drops;

    private void Start()
    {
        InitializeRegisters();
    }

    private void InitializeRegisters()
    {
        players = new GameObject[MAX_PLAYERS];
        enemies = new List<GameObject>();
        drops = new List<GameObject>();
    }

    public void RegisterPlayer(GameObject playerObject, int playerId)
    {
        players[playerId] = playerObject;
    }

    public void UnregisterPlayer(int playerId)
    {
        players[playerId] = null;
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

    public List<GameObject> GetDrops()
    {
        return enemies;
    }
}
