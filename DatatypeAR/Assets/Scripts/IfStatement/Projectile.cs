using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    void Update()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * 1f);
    }
}
