using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerManager))]
public class PlayerController : MonoBehaviour
{
    private PlayerManager playerManager;

    private Rigidbody2D rb;

    [SerializeField] private GameObject spriteContainer;

    [SerializeField] private float speed = 5f;
    [SerializeField] private Vector2 movementInput;

    void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (playerManager.HasPlayerObject())
        {

        }
    }

    public void OnAim(InputAction.CallbackContext ctx) => movementInput = ctx.ReadValue<Vector2>();

    public void OnMove()
    {
        
    }

    public void OnShoot()
    {

    }

    public void OnAbility()
    {

    }

    public void OnTether()
    {

    }
}
