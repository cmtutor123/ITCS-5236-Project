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
    public float bulletDamage;
    [SerializeField] private float bulletSpeed;
    public float moveSpeed = 5f;
    private float initialSpeed;
    
    private bool canShoot = true;
    Vector2 aimDirection = Vector2.zero;


    private void Awake()
    {
        input = new PlayerControls();
    }

    void Start()
    {
        initialSpeed = moveSpeed;
        moveSpeed = 0f;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        rb.velocity = new Vector2(aimDirection.x * moveSpeed, aimDirection.y * moveSpeed);
    }

    void Shoot(){
        canShoot = false;
        Debug.Log("Fire");
        Bullet _temp = Instantiate(bullet, transform.position, transform.rotation);
        _temp.GetComponent<Bullet>().setPlayerBullet(true);
        _temp.GetComponent<Bullet>().source = gameObject;
        _temp.GetComponent<Bullet>().damage = bulletDamage;
        _temp.GetComponent<Bullet>().speed = bulletSpeed;
    }

    public void MoveOnPerformed(InputAction.CallbackContext context)
    {
        moveSpeed = initialSpeed;
    }
    public void MoveOnCanceled(InputAction.CallbackContext context)
    {
        moveSpeed = 0f;
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
        Debug.Log("Tether");
    }
    public void TetherOnCanceled(InputAction.CallbackContext context)
    {
        Debug.Log("TetherStopped");
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
        if(canShoot)
        {
            Shoot();
            StartCoroutine(ShootDelay());
        }
        
    }
    IEnumerator ShootDelay()
    {
        yield return new WaitForSeconds(shootDelay);
        canShoot = true;
    }
}
