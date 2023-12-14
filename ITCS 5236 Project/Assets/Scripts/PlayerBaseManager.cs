using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBaseManager : MonoBehaviour
{
    private GameManager gameManager;

    private int currentResources;
    private int neededResources;
    private bool inWave;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        inWave = false;
    }

    private void Update()
    {
        if (inWave && currentResources >= neededResources)
        {
            EndWave();
        }
    }

    public void StartWave(int resources)
    {
        Debug.Log("Start Wave");
        Debug.Log("Resources Needed: " + resources);
        if (gameManager == null) gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        if (gameManager != null) gameManager.waveProgress = 0;
        currentResources = 0;
        neededResources = resources;
        inWave = true;
    }

    public void EndWave()
    {
        inWave = false;
        gameManager.EndRound();
    }

    public void EndGame()
    {
        gameManager.EndGame();
    }

    public void AddResources(int amount)
    {
        currentResources += Mathf.Clamp(amount, 0, int.MaxValue - currentResources);
        Debug.Log("AddResources() Called");
        if (gameManager == null) gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        if (gameManager != null) gameManager.waveProgress = (float)currentResources / neededResources;
        Debug.Log("Resources: " + currentResources + "/" + neededResources + " , Score: " + (gameManager == null ? "null" : gameManager.GetScore()));
        if (currentResources >= neededResources)
        {
            EndWave();
        }
    }
}
