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

    }

    public void AddResources(int amount)
    {
        currentResources += Mathf.Clamp(amount, 0, int.MaxValue - currentResources);
    }
}
