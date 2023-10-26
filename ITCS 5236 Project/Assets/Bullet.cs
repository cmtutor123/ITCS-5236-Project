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

    void OnCollisionEnter(Collision collision)
    {   
        if(collision.gameObject != source)
        {
            if (collision.gameObject.tag == "Enemy")
            {
                collision.gameObject.GetComponent<Health>().Damage(damage);
                Destroy(gameObject);
            }
            if (collision.gameObject.tag == "Player")
            {
                if(!playerBullet)
                    collision.gameObject.GetComponent<Health>().Damage(damage);
                if(playerBullet)
                    collision.gameObject.GetComponent<Rigidbody2D>().AddForce(transform.up * 1);
                Destroy(gameObject);
            }
            if (collision.gameObject.tag == "Base")
            {
                if(!playerBullet)
                    //A {} can be added here including the destroy(gameobject) if we want the player bullet to travel thru base
                    collision.gameObject.GetComponent<Health>().Damage(damage);
                Destroy(gameObject);
            }
        }
    }
    public void setPlayerBullet(bool playerBullet)
    {
        this.playerBullet = playerBullet;
    }
}

