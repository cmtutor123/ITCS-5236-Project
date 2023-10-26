using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    // get current 
    [SerializeField] private Transform myTransform;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private GameObject enemyToSpawn;
    [SerializeField] private GameObject dropPrefab;

    // for seeing if object is in camera view (screen)
    private Camera mainCamera;
    private MeshRenderer m_renderer;
    private Plane[] cameraFrustum;
    private BoxCollider2D m_collider;

    // change speed of change
    [SerializeField] private float maxSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float radiusOfSat;
    [SerializeField] private float damage;


    private Rigidbody2D rb;

    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); 

        // used to detect player on screen
        mainCamera = Camera.main;
        m_renderer = GetComponent<MeshRenderer>();
        m_collider = GetComponent<BoxCollider2D>();

        // get all plains
        cameraFrustum = GeometryUtility.CalculateFrustumPlanes(mainCamera);

    }

    // Move enemy to destination/create & destroy when moving off screen
    void Update()
    {        
        // Check if enemy is inside of bounds (screen)
        // If inside then move to object (using kinematic arrive)
        var bounds = m_collider.bounds;
        if(GeometryUtility.TestPlanesAABB(cameraFrustum, bounds)) {
            // calculate vector from character to target a
            Vector3 towardsTarget = targetTransform.position - myTransform.position;

            // If we haven't reached target then move in the direction towards target
            if(towardsTarget.magnitude > radiusOfSat) {
                // normalize vector (size of 1)
                towardsTarget.Normalize();
                towardsTarget *= maxSpeed;

                // turn towards target
                float angle = Mathf.Atan2(towardsTarget.y, towardsTarget.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
                myTransform.rotation = Quaternion.Lerp(myTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                // move enemy
                rb.AddForce(towardsTarget); 
            }
            else {
                rb.velocity = Vector2.zero;
            }
        }

        // if outside of bounds then destroy enemy object and place a new one somewhere else
        else {
            Destroy(this.gameObject);
            Debug.Log("Enemy outside of bounds");
             Instantiate(enemyToSpawn, myTransform.position, Quaternion.identity);
        }
    }

    // On collision with other objects do different actions
    // Player - minus health from player and destroy enemy
    // Drop - do nothing for now
    // 
    private void OnCollisionEnter2D(Collision2D other) {
        // if collide with drop then do nothing (for now)
        if(other.gameObject.tag == "Drop") {
            Debug.Log("Enemy collision with drop");
        }

        // if collide with player then destroy self (for now)
        if(other.gameObject.tag == "Player") {

            Destroy(this.gameObject);
            Debug.Log("Enemy collision with player");

            // create a drop at place of death
            Instantiate(dropPrefab, myTransform.position, Quaternion.identity);
        }


    }


}
