using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ControlsPlayer : MonoBehaviour
{
    private PlayerControls input = null;
    private Rigidbody2D rb;

    public float moveSpeed = 5f;
    private float initialSpeed;
    Vector2 aimDirection = Vector2.zero;


    private void Awake(){
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
    
    private void OnEnable(){
        input.Enable();
        input.Ship.Move.performed += MoveOnPerformed;
        input.Ship.Move.canceled += MoveOnCanceled;
        input.Ship.Aim.performed += AimOnPerformed;
        input.Ship.Aim.canceled += AimOnCanceled;
        input.Ship.Ability.performed += AbilityOnPerformed;
        input.Ship.Tether.performed += TetherOnPerformed;
        input.Ship.Tether.canceled += TetherOnCanceled;
        input.Ship.Pause.performed += PauseOnPerformed;
        input.Ship.Join.performed += JoinOnPerformed;
        input.Ship.Unjoin.performed += UnjoinOnPerformed;

    }
    private void OnDisable(){
        input.Disable();
        input.Ship.Move.performed -= MoveOnPerformed;
        input.Ship.Move.canceled -= MoveOnCanceled;
        input.Ship.Aim.performed -= AimOnPerformed;
        input.Ship.Aim.canceled -= AimOnCanceled;
        input.Ship.Ability.performed -= AbilityOnPerformed;
        input.Ship.Tether.performed -= TetherOnPerformed;
        input.Ship.Tether.canceled -= TetherOnCanceled;
        input.Ship.Pause.performed -= PauseOnPerformed;
        input.Ship.Join.performed -= JoinOnPerformed;
        input.Ship.Unjoin.performed -= UnjoinOnPerformed;
    }

    public void MoveOnPerformed(InputAction.CallbackContext context){
        moveSpeed = initialSpeed;
    }
    public void MoveOnCanceled(InputAction.CallbackContext context){
        moveSpeed = 0f;
    }
    public void AimOnPerformed(InputAction.CallbackContext context){
        aimDirection = context.ReadValue<Vector2>();
        Debug.Log("Aim: " + aimDirection);
    }
    public void AimOnCanceled(InputAction.CallbackContext context){
        aimDirection = Vector2.zero;
    }
    public void AbilityOnPerformed(InputAction.CallbackContext context){
        Debug.Log("Ability");
    }
    public void TetherOnPerformed(InputAction.CallbackContext context){
        Debug.Log("Tether");
    }
    public void TetherOnCanceled(InputAction.CallbackContext context){
        Debug.Log("TetherStopped");
    }
    public void PauseOnPerformed(InputAction.CallbackContext context){
        Debug.Log("Pause");
    }
    public void JoinOnPerformed(InputAction.CallbackContext context){
        Debug.Log("Join");
    }
    public void UnjoinOnPerformed(InputAction.CallbackContext context){
        Debug.Log("Unjoin");
    }

}
