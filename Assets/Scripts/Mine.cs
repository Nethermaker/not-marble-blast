using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Mines explode when touched by the player, moving the player around accoringly
public class Mine : MonoBehaviour {

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //Explosions are weird; they tend to send the player straight up, but with a crap ton of spin, making recovery difficult (which is good)
            GameObject player = collision.gameObject;
            Rigidbody rb = player.GetComponent<Rigidbody>();
            rb.AddExplosionForce(1000f, transform.position, 5f);

            Explode();
        }
    }

    //Controls the Particle system for explosions
    //Probably would have been more efficient for the particle system to be a separate game object that gets instantiated,
    //  rather than actually having the component be part of the mine itself.
    void Explode()
    {
        ParticleSystem exp = GetComponent<ParticleSystem>();
        exp.Play();
        Destroy(gameObject, exp.main.duration);
        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.enabled = false;
    }
}
