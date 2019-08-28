using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Launchers will launch the player in the direction that they are pointing when touched
public class Launcher : MonoBehaviour {

    public float force;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
            rb.velocity = transform.up * force;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
