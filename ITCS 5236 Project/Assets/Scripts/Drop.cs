using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : MonoBehaviour
{
    public int resources;
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
        Destroy(gameObject);
    }
}
