using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    // get current fields for enemy
    [SerializeField] private Transform myTransform;
    private Transform targetTransform;
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


    // for shooting (Player and Base Enemy)
    [SerializeField] private Bullet bullet;
    [SerializeField] float bulletDamage;
    [SerializeField] float bulletSpeed;
    [SerializeField] float impactDamage;
    [SerializeField] float shootDelay;
    private bool canShoot;


    // for tethering (DropEnemy)
    private int tetherAmount;
    private int maxTethers;
    private GameObject[] tethers;
    private GameObject tether;


    private Rigidbody2D rb;

    private GameManager gameManager;
    private GameObject jetfire;
    float tempX;
    float tempY;
    bool escape;

    private Cluster clusterAlgorith;
    
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        clusterAlgorith = GetComponent<Cluster>();

        if(gameObject.tag == "EnemyDrop") {
            targetTransform = clusterAlgorith.GetDestination().transform;
        } else {
            targetTransform = gameManager.targetTransform;
        }


        rb = GetComponent<Rigidbody2D>(); 

        // used to detect player on screen
        mainCamera = Camera.main;
        m_renderer = GetComponent<MeshRenderer>();
        m_collider = GetComponent<BoxCollider2D>();
        jetfire = transform.GetChild(0).gameObject;

        // get all plains
        cameraFrustum = GeometryUtility.CalculateFrustumPlanes(mainCamera);

        // allow enemy to shoot
        canShoot = true;

        // set up values for tethering
        tetherAmount = 3;
        maxTethers = tetherAmount;
        tethers = new GameObject[maxTethers];
        escape = false;

    }

    // Move enemy to destination/create & destroy when moving off screen
    void Update()
    {        
        // If target no longer exists attack something else.
        // Drop Enemy -> Leave screen
        // Player Enemy -> Get another live enemy
        // Base Enemy -> Game Over
        if (targetTransform == null)
        {
            GameObject playerObject = null;

            if(gameObject.tag == "EnemyDrop"){
                print("null is current targetTransform");
                //targetTransform = gameManager.GetDropTarget();
            }
            else if(gameObject.tag == "EnemyPlayer")
                playerObject = gameManager.GetPlayerTarget();


            if (playerObject != null) {
                targetTransform = playerObject.transform;
            }
        }
        
        /*if(targetTransform.tag == "Drop" && targetTransform.GetComponent<Tether>().tetheredTo != null) {
            targetTransform = null;
        }*/

        if(targetTransform == null /*&& gameObject.tag == "EnemyDrop"*/) {
            print("Escaping due to having no target transform");
            Escape();
        }


        // Check if enemy is inside of bounds (screen)
        // If inside then move to object (using kinematic arrive)
        var bounds = m_collider.bounds;
        if(InBounds()) {
            // calculate vector from character to target a
            Vector3 towardsTarget;
            if(!escape)
                towardsTarget = targetTransform.position - myTransform.position;
            else
                towardsTarget = (new Vector3(tempX, tempY, 0)) - myTransform.position;

            // If we haven't reached target then move in the direction towards target
            if(towardsTarget.magnitude > radiusOfSat) {
                // normalize vector (size of 1)
                towardsTarget.Normalize();
                

                // turn towards target
                jetfire.SetActive(true);
                Quaternion aimTowards = Quaternion.FromToRotation(Vector3.up, towardsTarget);
                myTransform.rotation =  Quaternion.Lerp(myTransform.rotation, aimTowards, rotationSpeed * Time.deltaTime);

                // move enemy
                towardsTarget *= maxSpeed;
                rb.AddForce(towardsTarget); 
            }

            // if enemy has reached destination then stop moving and start shooting
            else {
                rb.velocity = Vector2.zero;
                jetfire.SetActive(false);

                if(gameObject.tag == "EnemyPlayer") {
                    Quaternion aimTowards = Quaternion.FromToRotation(Vector3.up, towardsTarget);
                    myTransform.rotation =  Quaternion.Lerp(myTransform.rotation, aimTowards, rotationSpeed * Time.deltaTime);
                }


                if(gameObject.tag == "EnemyDrop") {
                    // Escape if tether amount is full
                    if(tetherAmount == 0) {
                        Escape();
                    }                         
                    // Escape if there is no drops on the screen
                    else if(gameManager.GetDrops().Count == 0) {
                        Escape();
                    }
                    // Tether if Target is Drop or Centroid
                    else if(targetTransform.tag == "Drop" || targetTransform.tag == "Centroid") {
                        print("In drop and centroid if statement");

                        //bool allTethered = true;
                        /*foreach(GameObject drop in gameManager.GetDrops()) {

                            if(!drop.GetComponent<Tether>().tethered) {
                                print("Drop made it in");
                                allTethered = false;
                            }
                        }
                        print("all tethered " + allTethered);
                        if(allTethered) {
                            Escape();
                        } else {*/
                        TetherOnPerformed();

                    } else {

                    }

                }
                else if(canShoot) {
                    //Debug.Log("Shooting");
                    Shoot();
                    StartCoroutine(ShootDelay());
                }
            }
        }

        // if outside of bounds then destroy enemy object and place a new one somewhere else
        else {
            foreach(GameObject tether in tethers){
                if(tether != null){
                    tether.GetComponent<Tether>().tethered = false;
                    tether.GetComponent<Tether>().tetheredTo = null;
                }
            }
            if(gameObject.tag == "EnemyDrop") {
                foreach(GameObject centroid in clusterAlgorith.GetCentroids()) {
                    Destroy(centroid);
                }
            }

            Destroy(gameObject);
            Debug.Log("Enemy outside of bounds");
        }
    }

    public bool InBounds()
    {
        if(gameObject.tag == "EnemyDrop") {
            return transform.position.x >= GameManager.BOUNDRY_X_MIN - 4 && transform.position.x <= GameManager.BOUNDRY_X_MAX + 4 && transform.position.y >= GameManager.BOUNDRY_Y_MIN - 4 && transform.position.y <= GameManager.BOUNDRY_Y_MAX + 4;
        } 
        
        return transform.position.x >= GameManager.BOUNDRY_X_MIN && transform.position.x <= GameManager.BOUNDRY_X_MAX && transform.position.y >= GameManager.BOUNDRY_Y_MIN && transform.position.y <= GameManager.BOUNDRY_Y_MAX;
        //return GeometryUtility.TestPlanesAABB(cameraFrustum, m_collider.bounds);
    }


    // Shoot a bullet in the direction enemy is facing
     void Shoot(){
        Vector3 towardsTarget = targetTransform.position - myTransform.position;
        towardsTarget.Normalize();
        Quaternion aimTowards = Quaternion.FromToRotation(Vector3.up, towardsTarget);
        myTransform.rotation =  Quaternion.Lerp(myTransform.rotation, aimTowards, rotationSpeed * Time.deltaTime);
        canShoot = false;
        Bullet _temp = Instantiate(bullet, transform.position, transform.rotation);
        _temp.GetComponent<Bullet>().setPlayerBullet(false);
        _temp.GetComponent<Bullet>().source = gameObject;
        _temp.GetComponent<Bullet>().damage = bulletDamage;
        _temp.GetComponent<Bullet>().speed = bulletSpeed;
    }

    // cooldown for shooting
    IEnumerator ShootDelay()
    {
        yield return new WaitForSeconds(shootDelay);
        canShoot = true;
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

            if(gameObject.tag == "EnemyDrop") {

            }

            foreach(GameObject tether in tethers){
                if(tether != null){
                    tether.GetComponent<Tether>().tethered = false;
                    tether.GetComponent<Tether>().tetheredTo = null;
                }
            }
            if(gameObject.tag == "EnemyDrop") {
                foreach(GameObject centroid in clusterAlgorith.GetCentroids()) {
                    Destroy(centroid);
                }
            }

            Destroy(gameObject);

            // create a drop at place of death
            // Instantiate(dropPrefab, myTransform.position, Quaternion.identity);
            other.gameObject.GetComponent<Health>().Damage(impactDamage);
        }


    }

    public void onDeath(){
        // create a drop at place of death
        Instantiate(dropPrefab, myTransform.position, Quaternion.identity);
        foreach(GameObject tether in tethers){
            if(tether != null){
                tether.GetComponent<Tether>().tethered = false;
                tether.GetComponent<Tether>().tetheredTo = null;
            }
        }
        if(gameObject.tag == "EnemyDrop") {
            foreach(GameObject centroid in clusterAlgorith.GetCentroids()) {
                Destroy(centroid);
            }
        }

        Destroy(gameObject);
    }

    public void TetherOnPerformed()
    {
        // check if there you have teathers left and grab drop object
        if(tetherAmount > 0)
        {
            foreach(RaycastHit2D hit in Physics2D.CircleCastAll(gameObject.transform.position, 5f, Vector2.zero))
            {
                if(hit.collider.gameObject.tag == "Drop" && tetherAmount > 0 && !hit.collider.gameObject.GetComponent<Tether>().tethered)
                {
                    tethers.SetValue(hit.collider.gameObject, tetherAmount-1);
                    tether = hit.collider.gameObject;
                    tether.GetComponent<Tether>().tethered = true;
                    tether.GetComponent<Tether>().tetheredTo = gameObject;
                    tetherAmount--;
                    targetTransform = null;
                    print("Tether");
                }
            }
        }
        else
        {
            Debug.Log("No Tethers Left");
            Escape();
        }
    }
    void Escape(){
        Debug.Log("Escaping");
        // set destination to leave screen (destroys self and drops)
        float enemyXPosition = transform.position.x;
        float enemyYPosition = transform.position.y;
        tempX = enemyXPosition;
        tempY = enemyYPosition;
        if(Mathf.Abs(enemyXPosition - GameManager.BOUNDRY_X_MIN) < Mathf.Abs(enemyXPosition - GameManager.BOUNDRY_X_MAX)) {
            tempX = GameManager.BOUNDRY_X_MIN - 12;
        }
        else {
            tempX = GameManager.BOUNDRY_X_MAX + 12;
        }

        if(Mathf.Abs(enemyYPosition - GameManager.BOUNDRY_Y_MIN) < Mathf.Abs(enemyYPosition - GameManager.BOUNDRY_Y_MAX)) {
            tempY = GameManager.BOUNDRY_Y_MIN - 12;
        }
        else {
            tempY = GameManager.BOUNDRY_Y_MAX + 12;
        }
        escape = true;
    }
}
