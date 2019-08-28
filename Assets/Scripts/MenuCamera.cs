using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This simple script is the one used on the main menu to orbit the camera slowly around the player
public class MenuCamera : MonoBehaviour {

    public Transform player;
    public float speed;

    private Vector3 offset = new Vector3(0, -1.5f, 7);

    private void Update()
    {
        offset = Quaternion.AngleAxis(Time.deltaTime * speed, Vector3.up) * offset;

        transform.position = player.position - offset;
        transform.LookAt(player.position);
    }
}
