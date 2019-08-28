using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script moves an object back and forth between two points, and takes the player with it
public class MovingPlatform : MonoBehaviour {

    public Vector3 startTransform;
    public Vector3 endTransform;

    private Vector3 currentPosition;
    private Vector3 lastPosition;
    private Vector3 movement;

    private bool playerColliding;
    private GameObject player;

    public float speed;

    private void Start()
    {
        player = GameObject.Find("Player");
    }

    private void FixedUpdate()
    {
        currentPosition = transform.position;
        movement = currentPosition - lastPosition;

        //If the player's movement is not adjusted while on a platform, it causes strange bouncing issues when/if the platform descends
        //As a result, some funkiness can occur when getting on/off a platform, but if it's not there, stuff gets even funkier
        //As far as I can tell, this isn't why the platforms don't impart force on the player (I believe that's because the platforms aren't moved using their rigidbody)
        if (playerColliding)
        {
            player.transform.position += movement;
        }

        float t = (Mathf.Sin(Time.time / speed) + 1f) / 2f;

        transform.position = new Vector3(Mathf.Lerp(startTransform.x, endTransform.x, t),
                                         Mathf.Lerp(startTransform.y, endTransform.y, t),
                                         Mathf.Lerp(startTransform.z, endTransform.z, t));

        lastPosition = currentPosition;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Player")
        {
            playerColliding = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "Player")
        {
            playerColliding = false;
        }
    }
}
