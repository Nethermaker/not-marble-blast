using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Fans push the player away from them while they are within their influence
public class Fan : MonoBehaviour {

    public float force = 10f;
    private bool playerIsColliding = false;
    private Rigidbody rb;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        rb = player.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (playerIsColliding)
        {
            //This doesn't work when you throw rotators into the mix, but I couldn't figure out how to fix it
            rb.AddForce(Quaternion.Euler((Vector3.up) * 90f) * transform.forward * force);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerIsColliding = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerIsColliding = false;
        }
    }
}
