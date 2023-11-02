using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : MonoBehaviour
{
    public int resources;
    private Tether tether;
    void Start(){
        tether = gameObject.GetComponent<Tether>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Base")
        {
            other.gameObject.GetComponent<PlayerBaseManager>().AddResources(resources);
            Destroy(gameObject);
        }
    }

    void OnBecameInvisible()
    {
        if(!tether.tethered)
            Destroy(gameObject);
    }
}
