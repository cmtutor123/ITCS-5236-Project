using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    // get current 
    [SerializeField] private Transform myTransform;
    [SerializeField] private Transform targetTransform;

    // change speed of change
    [SerializeField] private float maxSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float radiusOfSat;

    private Rigidbody2D rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();   
    }

    // Update is called once per frame
    void Update()
    {
        // calculate vector from character to target a
        Vector3 towardsTarget = targetTransform.position - myTransform.position;
        float angle = Mathf.Atan2(towardsTarget.y, towardsTarget.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        myTransform.rotation = Quaternion.Lerp(myTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        //Quaternion.AngleAxis(angle, Vector3.forward);


        // If we haven't reached target then move in the direction towards target
        if(towardsTarget.magnitude > radiusOfSat) {
            // normalize vector (size of 1)
            towardsTarget.Normalize();
            towardsTarget *= maxSpeed;

            // move enemy
            rb.AddForce(towardsTarget); 
        }
        else {
            rb.velocity = Vector2.zero;
        }
    }
}
