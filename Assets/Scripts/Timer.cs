using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This should be relatively self-explanatory
//This is just a timer that times how quickly you complete a level, and can be slowed by the time travel powerup
public class Timer : MonoBehaviour {

    private float timer;
    private float timeModifier = 1;
    private PlayerController pc;

    public bool timeTravelActive = false;
    public int timeTravels = 0;

    private Text timerText;

    private void Start()
    {
        pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        timerText = GetComponent<Text>();
    }

    private void Update()
    {
        if (!pc.isPaused && !pc.levelEnded)
        {
            timer += (Time.deltaTime * 1000) / timeModifier;

            TimeSpan ts = TimeSpan.FromMilliseconds(timer);

            timerText.text = string.Format("{0:00}:{1:00}.{2:000}", ts.Minutes, ts.Seconds, ts.Milliseconds);
        }
        else if (pc.levelEnded)
        {
            timerText.color = new Color(255, 255, 255);
        }
    }

    public IEnumerator TimeTravel()
    {
        timeTravelActive = true;
        timeModifier = 10;
        timerText.color = new Color(0, 255, 0);

        while (timeTravels > 0)
        {
            yield return new WaitForSeconds(10);
            timeTravels--;
        }

        timeTravelActive = false;
        timeModifier = 1;
        timerText.color = new Color(25, 255, 255);
    }
}
