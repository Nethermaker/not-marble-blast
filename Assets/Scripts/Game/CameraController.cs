using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script controls the camera and orbits it around the player based on mouse movement
public class CameraController : MonoBehaviour {

    public Transform player;
    public float turnSpeedX;
    public float turnSpeedY;

    public bool positionLocked = false;

    private Vector3 offset = new Vector3(0, -1.5f, 4);

    private PlayerController playerController;

    private void Start()
    {
        //Makes a reference to the playerController script so it knows when the game is paused, level is finished, or the player is out of bounds.
        playerController = player.GetComponent<PlayerController>();
    }

    private void LateUpdate()
    {
        //Only works if level isn't over, paused, or player is out of bounds
        if (!playerController.levelEnded && !playerController.isPaused && !positionLocked)
        {
            //Gets both axes of difference of mouse movement since last frame and puts them in floats
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            //Camera can always be rotated along x axis
            offset = Quaternion.AngleAxis(mouseX * turnSpeedX, Vector3.up) * offset;

            //Camera can only be rotated along y axis if it's not already too high or low
            if (((transform.rotation.eulerAngles.x > 80 && transform.rotation.eulerAngles.x < 90) && mouseY < 0) 
                || ((transform.rotation.eulerAngles.x < 280 && transform.rotation.eulerAngles.x > 270) && mouseY > 0))
            {
                //Do nothing. Prevents camera from going too high or low
            }
            else
            {
                offset = Quaternion.AngleAxis(-mouseY * turnSpeedY, transform.right) * offset;
            }

            transform.position = player.position - offset;
        }
        transform.LookAt(player.position);
    }
}
