using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script moves an object back and forth between two points, and takes the player with it
public class MovingPlatform : MonoBehaviour {

    public Vector3 startTransform;
    public Vector3 endTransform;

    private Vector3 currentPosition;
    private Vector3 lastPosition;
    private Rigidbody rb;

    public float speed;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        currentPosition = transform.position;

        float t = (Mathf.Sin(Time.time / speed) + 1f) / 2f;

        currentPosition = new Vector3(Mathf.Lerp(startTransform.x, endTransform.x, t),
                                         Mathf.Lerp(startTransform.y, endTransform.y, t),
                                         Mathf.Lerp(startTransform.z, endTransform.z, t));
        rb.MovePosition(currentPosition);

        lastPosition = currentPosition;
    }
}
