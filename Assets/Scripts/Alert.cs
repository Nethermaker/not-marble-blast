using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This script displays messages to the player when they enter a trigger
public class Alert : MonoBehaviour {

    public string alert;
    public Text alertText;

    private void Start()
    {
        alertText = GameObject.Find("Alert Text").GetComponent<Text>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            alertText.text = alert;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            alertText.text = "";
        }
    }
}
