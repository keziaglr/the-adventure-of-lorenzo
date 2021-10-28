using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonShooting : MonoBehaviour
{
    public GameObject bullet;
    public Transform point;
    private float startTime, timeElapsed;

    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        timeElapsed = Time.time - startTime;
        if (timeElapsed >= 3)
        {
            shootCannon();
            startTime = Time.time;
        }
    }

    void shootCannon()
    {
        GameObject o = Instantiate(bullet, point.position, Quaternion.identity);
        o.transform.rotation = point.rotation;
        Rigidbody rb = o.GetComponent<Rigidbody>();
        rb.AddForce(o.transform.forward * 1000);
    }
}
