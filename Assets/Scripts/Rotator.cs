using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The rotator rotates the level around the player, allowing for some very interesting level design.
//Originally the plan was to make this function similary to the gravity modifier in marble blast (which actually changes gravity), but that didn't work here for several reasons
//1) It probably could work but I'm not smart enough
//2) A lot of things depend on the level being right-side up
//3) The way the camera and player movement are controlled, changing gravity (which can be done easily) would cause all sorts of funkiness I couldn't imagine how to fix

//Then I decided to "fake" a gravity change by rotating the skybox and the level's directional light, but this also did not work
//The skybox can't actually be rotated, so what you can do is create a camera that only can see the skybox, layer it on top of the main camera, and rotate that.
//This "worked", but not in the way I expected it to, so I threw the idea out eventually
public class Rotator : MonoBehaviour {

    public GameObject level;
    public GameObject levelPivot;
    public GameObject player;
    public Vector3 rotation;

    public bool isVisible;
    public bool isActive = true;

    //Gets references to things that it needs
    private void Start()
    {
        level = GameObject.Find("Level");
        levelPivot = GameObject.Find("Level Pivot");
        player = GameObject.Find("Player");

        if (!isVisible)
        {
            foreach (Transform child in transform)
            {
                MeshRenderer mr = child.gameObject.GetComponent<MeshRenderer>();
                mr.enabled = false;
            }
        }
    }

    //This is all visual stuff
    private void Update()
    {
        foreach (Transform child in transform)
        {
            child.transform.Rotate(transform.up * Time.deltaTime * 50, Space.World);

            MeshRenderer mr = child.gameObject.GetComponent<MeshRenderer>();
            if (isActive)
            {
                mr.enabled = true;
            }
            else
            {
                mr.enabled = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && isActive)
        {
            player.GetComponent<PlayerController>().usedRotators.Add(gameObject);
            StartCoroutine("RotateLevel");
        }
    }

    IEnumerator RotateLevel()
    {
        PlayerController pc = player.GetComponent<PlayerController>();
        pc.canReset = false;
        levelPivot.transform.position = transform.position;

        level.transform.SetParent(levelPivot.transform, true);
        player.transform.SetParent(levelPivot.transform, true);

        if (isVisible)
        {
            //Rotating the level instantly without any accompanying visuals can be... disorienting, so I added this to do it over the course of one second
            Time.timeScale = 0;
            Vector3 newRotation = rotation / 60;
            for (int i = 1; i <= 60; i++)
            {
                levelPivot.transform.Rotate(newRotation);
                yield return null;
            }
            Time.timeScale = 1;
        }
        else
        {
            //If the rotator isn't visible (every checkpoint has an invisible rotator), then rotate the level instantly)
            //This needs to be done so the level is always right-way-up when you reset
            levelPivot.transform.rotation = Quaternion.Euler(rotation);
        }

        player.transform.SetParent(null, true);
        level.transform.SetParent(null, true);

        isActive = false;
        pc.canReset = true;
    }
}
