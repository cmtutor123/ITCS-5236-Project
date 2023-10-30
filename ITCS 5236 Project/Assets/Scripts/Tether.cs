using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tether : MonoBehaviour
{
    // Start is called before the first frame update
    public bool tethered;
    private Rigidbody2D rb;
    public GameObject tetheredTo;
    public float maxMoveSpeed;
    public float tetherSpeed;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        LineRenderer lr = gameObject.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.widthMultiplier = 0.05f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.cyan, 0.0f), new GradientColorKey(Color.blue, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
        );
        lr.colorGradient = gradient;
    }

    // Update is called once per frame
    void Update()
    {
        LineRenderer lr = GetComponent<LineRenderer>();
        if(tethered){
            lr.positionCount = 2;
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, tetheredTo.transform.position);
            if (rb.velocity.magnitude > maxMoveSpeed)
            {
                rb.velocity = rb.velocity.normalized * maxMoveSpeed;
            }
            if(Vector2.Distance(transform.position, tetheredTo.transform.position) > 2)
                rb.AddForce((tetheredTo.transform.position - transform.position).normalized * tetherSpeed);
            if(Vector2.Distance(transform.position, tetheredTo.transform.position) > 0.5)
                rb.velocity = rb.velocity * 0.97f;
        } else {
            lr.positionCount = 0;
            rb.velocity = rb.velocity * 0.97f;
        }
    }
}
