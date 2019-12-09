using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script controls whether or not a powerup can be picked up, and changes its visibility accordingly
//Default cooldown on powerup pickup is 10s but this can be changed (and is for the gyrocopter)
public class Powerup : MonoBehaviour {

    public float rechargeTime = 10f;
    public bool isActive = true;

    private float timeDeactivated;

    private MeshRenderer mr;

    private GameObject timer;

    private void Start()
    {
        mr = GetComponent<MeshRenderer>();

        if (gameObject.name == "Time Travel")
        {
            timer = GameObject.Find("Timer");
        }
    }

    public void Deactivate()
    {
        timeDeactivated = Time.time;
        isActive = false;
        mr.enabled = false;
    }

    public void Update()
    {
        if (Time.time - timeDeactivated >= rechargeTime)
        {
            isActive = true;
            mr.enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (timer != null)
        {
            Timer timerScript = timer.GetComponent<Timer>();
            if (timerScript.timeTravelActive == false)
            {
                timerScript.timeTravels++;
                timerScript.StartCoroutine("TimeTravel");
            }
            else
            {
                timerScript.timeTravels++;
            }
            Destroy(gameObject);
        }
    }
}
