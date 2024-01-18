using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerControls input = null;
    private Rigidbody2D rb;
    public float shootDelay;
    [SerializeField] private Bullet bullet;
    private GameObject jetfire;
    public float bulletDamage;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private int tetherAmount;
    private int maxTethers;
    private GameObject[] tethers;
    public float moveSpeed = 5f;
    public float maxMoveSpeed = 5f;
    private float initialSpeed;
    public float turnSpeed;
    
    private bool canShoot = true;
    Vector2 aimDirection = Vector2.zero;
    private GameObject tether;

    private bool canMove = false;
    private bool isShooting = false;

    public PlayerManager playerManager;

    public Dictionary<string, float> stats = new Dictionary<string, float>();

    public Color shipColor;

    private void Awake()
    {
        input = new PlayerControls();
    }

    void Start()
    {
        initialSpeed = moveSpeed;
        maxTethers = tetherAmount;
        tethers = new GameObject[maxTethers];
        rb = GetComponent<Rigidbody2D>();
        jetfire = transform.GetChild(0).GetChild(1).GetChild(0).gameObject;
    }
    void Update(){
        if(isShooting)
            Shoot();
    }

    void FixedUpdate()
    {
        if (transform.position.x < GameManager.BOUNDRY_X_MIN)
        {
            transform.position = new Vector2(GameManager.BOUNDRY_X_MAX, transform.position.y);
        }
        if (transform.position.x > GameManager.BOUNDRY_X_MAX)
        {
            transform.position = new Vector2(GameManager.BOUNDRY_X_MIN, transform.position.y);
        }
        if (transform.position.y < GameManager.BOUNDRY_Y_MIN)
        {
            transform.position = new Vector2(transform.position.x, GameManager.BOUNDRY_Y_MAX);
        }
        if (transform.position.y > GameManager.BOUNDRY_Y_MAX)
        {
            transform.position = new Vector2(transform.position.x, GameManager.BOUNDRY_Y_MIN);
        }
        if(aimDirection != Vector2.zero)
        {
            Vector2 _temp = aimDirection;
            _temp.Normalize();
            transform.up = Vector2.Lerp(transform.up, _temp, turnSpeed);
        }
        if (rb.velocity.magnitude > maxMoveSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxMoveSpeed;
        }
        if (canMove)
        {
            jetfire.SetActive(true);
            rb.AddForce(transform.up * moveSpeed);
        } else {
            jetfire.SetActive(false);
            rb.velocity = rb.velocity * 0.9f;
        }
    }

    void Shoot(){
            if(canShoot){
                canShoot = false;
                //Debug.Log("Fire");
                Bullet _temp = Instantiate(bullet, transform.position, transform.rotation);
                _temp.GetComponent<Bullet>().setPlayerBullet(true);
                _temp.GetComponent<Bullet>().source = gameObject;
                _temp.GetComponent<Bullet>().damage = bulletDamage;
                _temp.GetComponent<Bullet>().speed = bulletSpeed;
                StartCoroutine(ShootDelay());
            }
    }

    public void MoveOnPerformed(InputAction.CallbackContext context)
    {
        moveSpeed = initialSpeed;
    }

    public void MoveOnStarted(InputAction.CallbackContext context)
    {
        canMove = true;
    }

    public void MoveOnCanceled(InputAction.CallbackContext context)
    {
        canMove = false;
    }
    public void AimOnPerformed(InputAction.CallbackContext context)
    {
        aimDirection = context.ReadValue<Vector2>();
        //Debug.Log("Aim: " + aimDirection);
    }
    public void AimOnCanceled(InputAction.CallbackContext context)
    {
        aimDirection = Vector2.zero;
    }
    public void AbilityOnPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Ability");
    }
    public void TetherOnPerformed(InputAction.CallbackContext context)
    {
        if(tetherAmount > 0)
        {
            if (transform == null) return;
            foreach(RaycastHit2D hit in Physics2D.CircleCastAll(transform.position, 5f, Vector2.zero))
            {
                if(hit.collider.gameObject.tag == "Drop" && tetherAmount > 0 && !hit.collider.gameObject.GetComponent<Tether>().tethered)
                {
                    tethers.SetValue(hit.collider.gameObject, tetherAmount-1);
                    tether = hit.collider.gameObject;
                    tether.GetComponent<Tether>().tethered = true;
                    tether.GetComponent<Tether>().tetheredTo = gameObject;
                    tetherAmount--;
                }
            }

        }
        else
        {
            Debug.Log("No Tethers Left");
        }
    }
    public void TetherOnCanceled(InputAction.CallbackContext context)
    {
        //Debug.Log("Tether Canceled");
        tetherAmount = maxTethers;
        foreach(GameObject tether in tethers)
        {
            if(tether != null)
            {
                tether.GetComponent<Tether>().tethered = false;
                tether.GetComponent<Tether>().tetheredTo = null;
            }
        }

    }
    public void PauseOnPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Pause");
    }
    public void JoinOnPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Join");
    }
    public void UnjoinOnPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Unjoin");
    }
    public void ShootOnPerformed(InputAction.CallbackContext context)
    {
        isShooting = true;
    }
    public void ShootOnCanceled(InputAction.CallbackContext context)
    {
        isShooting = false;
    }
    public void ChangeOnPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Change");
    }

    public void SelectOnPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Select");
    }

    IEnumerator ShootDelay()
    {
        yield return new WaitForSeconds(shootDelay);
        canShoot = true;
    }

    public void ShipDestroyed()
    {
        if (playerManager != null) playerManager.RespawnPlayer();
        Destroy(gameObject);
        if (tether != null) Destroy(tether);
    }

    public void UpdateStats(Dictionary<string, float> stats)
    {
        Debug.Log("Update Controller Stats");
        this.stats = stats;
        UpdateShipStats();
    }

    public void UpdateShipStats()
    {
        Debug.Log("Update Ship Stats");
        foreach (string stat in stats.Keys)
        {
            Debug.Log(stat + ": " + stats[stat]);
        }
        bulletDamage = GetStat("damage");
        shootDelay = 1 / GetStat("fireRate");
        Health health = GetComponent<Health>();
        if (health != null)
        {
            health.SetMaxHealth(GetStat("health"));
            health.SetRegen(GetStat("regen"));
            health.SetDamageMultiplier(GetStat("damageResist"));
        }
        maxTethers = (int) GetStat("tetherCount");
        initialSpeed = GetStat("thrust");
        moveSpeed = GetStat("thrust");
        maxMoveSpeed = GetStat("maxSpeed");
    }

    public float GetStat(string stat)
    {
        Debug.Log("Getting Stat: " + stat);
        if (stats.ContainsKey(stat))
        {
            Debug.Log("Key Found");
            return stats[stat];
        }
        else
        {
            Debug.Log("Key Not Found");
            return 0;
        }
    }

    private void OnDestroy()
    {
        playerManager.hasPlayerObject = false;
    }

    public void SetColor(Color color)
    {
        shipColor = color;
        transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().color = shipColor;
    }
}
