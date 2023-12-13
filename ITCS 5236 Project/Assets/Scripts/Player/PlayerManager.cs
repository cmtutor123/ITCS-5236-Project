using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    private GameManager gameManager;

    [SerializeField] private GameObject prefabPlayerShip;

    private GameObject playerShip = null;

    private PlayerController playerShipController;

    private int playerId;

    private bool hasPlayerObject;

    private bool isReady = false;
    private bool onPlayerSelect = true;

    private UIControl uiControl;

    private float respawnTime = 3f;

    private PlayerClass playerClass;

    public List<Upgrade> possibleUpgrades = new List<Upgrade>();
    public List<Upgrade> selectedUpgrades = new List<Upgrade>();
    public List<Upgrade> lockedUpgrades = new List<Upgrade>();
    public List<Upgrade> upgradesAll = new List<Upgrade>();
    public List<Upgrade> upgradeSelection = new List<Upgrade>();

    public Dictionary<string, float> stats = new Dictionary<string, float>();

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        gameManager.RegisterPlayer(this);
        uiControl = GameObject.FindGameObjectWithTag("PlayerUI").GetComponent<UIControl>();
    }

    void Update()
    {
        
    }

    public GameObject GetPlayerShip()
    {
        return playerShip;
    }

    public bool HasPlayerObject()
    {
        return hasPlayerObject;
    }

    public void SetPlayerId(int id)
    {
        playerId = id;
    }

    public int GetPlayerId()
    {
        return playerId;
    }

    public void SpawnPlayer()
    {
        playerShip = Instantiate(prefabPlayerShip);
        playerShipController = playerShip.GetComponent<PlayerController>();
        playerShipController.playerManager = this;
        playerShipController.UpdateShipStats();
        hasPlayerObject = true;
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        //Debug.Log("Aim Button Pressed");
        if (hasPlayerObject) playerShipController.AimOnPerformed(context);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (false && context.performed)
        {
            //Debug.Log("Move Button Pressed");
            if (hasPlayerObject) playerShipController.MoveOnPerformed(context);
        }
        else if (context.started)
        {
            //Debug.Log("Move Button Started");
            if (hasPlayerObject) playerShipController.MoveOnStarted(context);
        }
        else if (context.canceled)
        {
            //Debug.Log("Move Button Canceled");
            if (hasPlayerObject) playerShipController.MoveOnCanceled(context);
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        Debug.Log("Shoot Button Pressed");
        if(context.performed){
            if (hasPlayerObject) playerShipController.ShootOnPerformed(context);
        }
        if(context.canceled){
            if (hasPlayerObject) playerShipController.ShootOnCanceled(context);
        }
    }

    public void OnAbility(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Ability Button Pressed");
            if (hasPlayerObject) playerShipController.AbilityOnPerformed(context);
        }
    }

    public void OnTether(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Tether Button Pressed");
            if (hasPlayerObject) playerShipController.TetherOnPerformed(context);
        }
        if(context.canceled){
            if (hasPlayerObject) playerShipController.TetherOnCanceled(context);
        }
    }

    public void OnButtonSelect(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Select Button Pressed");
            if (onPlayerSelect && !isReady) ReadyPlayer();
        }
    }

    public void OnButtonBack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Back Button Pressed");
            if (onPlayerSelect && isReady) UnreadyPlayer();
            else if (onPlayerSelect && !isReady) UnjoinPlayer();
        }
    }

    public void OnButtonChange(InputAction.CallbackContext context)
    {
        return;
		if (context.performed)
        {
            //Debug.Log("Change Button Pressed");
            if (onPlayerSelect && !isReady) ChangeShip(context.ReadValue<Vector2>().x);
        }
    }

    public void UnjoinPlayer()
    {
        uiControl.unjoinPlayer(playerId);
        gameManager.UnregisterPlayer(this);
        Destroy(gameObject);
    }

    public void UnreadyPlayer()
    {
        isReady = false;
        uiControl.unreadyPlayer(playerId);
    }

    public void ReadyPlayer()
    {
        isReady = true;
        uiControl.readyPlayer(playerId);
    }

    public void ChangeShip(float direction)
    {
        uiControl.PlayerChangeShips(playerId, direction);
    }

    public void Death()
    {
        gameManager.UnregisterPlayer(this);
        Destroy(gameObject);
    }

    public void RespawnPlayer()
    {
        StartCoroutine(RespawnShip());
    }

    public IEnumerator RespawnShip()
    {
        yield return new WaitForSeconds(respawnTime);
        if (GameManager.inRound)
        {
            SpawnPlayer();
        }
    }

    public void DestroyPlayer()
    {
        if (hasPlayerObject)
        {
            Destroy(playerShip);
            playerShip = null;
            hasPlayerObject = false;
        }
    }

    public float GetHealth()
    {
        if (hasPlayerObject)
        {
            if (playerShip != null)
            {
                Health health = playerShip.GetComponent<Health>();
                if (health != null)
                {
                    return health.GetHealth();
                }
            }
        }
        return 0;
    }

    public void InitializeUpgradeLists()
    {
        foreach (Upgrade upgrade in upgradesAll)
        {
            if (upgrade.requirements == null || upgrade.requirements.Count == 0)
            {
                possibleUpgrades.Add(upgrade);
            }
            else
            {
                lockedUpgrades.Add(upgrade);
            }
        }
    }

    public void SelectUpgrade(int index)
    {
        if (index < upgradeSelection.Count)
        {
            selectedUpgrades.Add(upgradeSelection[index]);
            upgradeSelection.RemoveAt(index);
        }
        else if (upgradeSelection.Count != 0)
        {
            selectedUpgrades.Add(upgradeSelection[0]);
            upgradeSelection.RemoveAt(0);
        }
        while (selectedUpgrades.Count > 0)
        {
            possibleUpgrades.Add(upgradeSelection[0]);
            upgradeSelection.RemoveAt(0);
        }
        CheckUpgradeRequirements();
    }

    public void CheckUpgradeRequirements()
    {
        for (int i = lockedUpgrades.Count - 1; i >= 0; i--)
        {
            Upgrade current = lockedUpgrades[i];
            bool met = true;
            foreach (Upgrade requirement in current.requirements)
            {
                if (!selectedUpgrades.Contains(requirement))
                {
                    met = false;
                    break;
                }
            }
            if (met)
            {
                lockedUpgrades.Remove(current);
                possibleUpgrades.Add(current);
            }
        }
    }

    public List<Upgrade> GetUpgradeSelection()
    {
        while (upgradeSelection.Count < 3 && possibleUpgrades.Count > 0)
        {
            Upgrade selection = possibleUpgrades[Random.Range(0, possibleUpgrades.Count)];
            possibleUpgrades.Remove(selection);
            upgradeSelection.Add(selection);
        }
        return upgradeSelection;
    }

    public void UpdateStats()
    {
        stats.Clear();
        List<(string, float)> classStats = playerClass.GetStats();
        foreach((string, float) stat in classStats)
        {
            stats.Add(stat.Item1, stat.Item2);
        }
        foreach(Upgrade upgrade in selectedUpgrades)
        {
            if (upgrade.stats == null || upgrade.modifiers == null || upgrade.stats.Count == 0 || upgrade.modifiers.Count == 0 || upgrade.stats.Count != upgrade.modifiers.Count) { }
            else
            {
                for (int i = 0; i < upgrade.stats.Count; i++)
                {
                    string stat = upgrade.stats[i];
                    if (!stats.ContainsKey(stat))
                    {
                        stats.Add(stat, 1);
                    }
                    stats[stat] *= upgrade.modifiers[i];
                }
            }
        }
        if (hasPlayerObject && playerShipController != null)
        {
            playerShipController.UpdateStats(stats);
        }
    }
}
