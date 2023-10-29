using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Start is called before the first frame update
    public float damage;
    public float speed;
    public float aliveTime;
    public bool playerBullet;

    [SerializeField] private GameObject dropPrefab;
    public GameObject source;
    void Start()
    {
        Destroy(gameObject, aliveTime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {   
        if(collision.gameObject != source)
        {
            if (collision.tag == "EnemyBase" || collision.tag == "EnemyPlayer" || collision.tag == "EnemyDrop" )
            {
                Debug.Log("Bullet hit Enemy");
                collision.gameObject.GetComponent<Health>().Damage(damage);
                Debug.Log(collision.gameObject.GetComponent<Health>().GetHealth());
                Destroy(gameObject);
            }
            if (collision.tag == "Player")
            {
                Debug.Log("Bullet hit Player");
                if(!playerBullet)
                    collision.gameObject.GetComponent<Health>().Damage(damage);
                if(playerBullet)
                    collision.gameObject.GetComponent<Rigidbody2D>().AddForce(transform.up * 1);
                Destroy(gameObject);
            }
            if (collision.tag == "Base")
            {
                Debug.Log("Bullet hit base");
                if(!playerBullet){
                    //A {} can be added here including the destroy(gameobject) if we want the player bullet to travel thru base
                    collision.gameObject.GetComponent<Health>().Damage(damage);
                    Destroy(gameObject);
                }
            }
        }
    }
    public void setPlayerBullet(bool playerBullet)
    {
        this.playerBullet = playerBullet;
    }
}

