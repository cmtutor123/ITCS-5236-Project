using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyMovement : MonoBehaviour
{
    [SerializeField] private Transform myTransform;

    private Vector3 targetVector;

    
    // change speed of change
    [SerializeField] private float maxSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float radiusOfSat;

    private Rigidbody2D rb;
    private Cluster clusterAlgorith;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); 


        clusterAlgorith = GetComponent<Cluster>();
    }

    // Update is called once per frame
    void Update()
    {
        // move towards target location
        targetVector = clusterAlgorith.GetDestination().transform.position;

        Vector3 towardsTarget = targetVector - myTransform.position;

        // Check if we are within satisfication range. 
            // If we are then turn and move towards target
            // Else stop moving
        if(towardsTarget.magnitude > radiusOfSat) {
            // normalize vector (size of 1)
            towardsTarget.Normalize();
                
            // turn towards target
            Quaternion aimTowards = Quaternion.FromToRotation(Vector3.up, towardsTarget);
            myTransform.rotation =  Quaternion.Lerp(myTransform.rotation, aimTowards, rotationSpeed * Time.deltaTime);

            // move enemy
            towardsTarget *= maxSpeed;
            rb.AddForce(towardsTarget); 

        } else {
            // stop moving
            rb.velocity = Vector2.zero;
        }
    }

}
