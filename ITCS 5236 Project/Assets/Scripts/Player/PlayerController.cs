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
                Debug.Log("Fire");
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
        Debug.Log("Aim: " + aimDirection);
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
        Debug.Log("Tether Canceled");
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
}
