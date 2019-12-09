using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//These get used in exactly one level (and so do the fans now that I think about it)
//When the player touches a trapdoor, it drops out from underneath them for a few seconds before returning to its original position
public class Trapdoor : MonoBehaviour {

    private bool isActive = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isActive)
        {
            StartCoroutine("Activate");
        }
    }

    IEnumerator Activate()
    {
        isActive = true;

        yield return new WaitForSeconds(1);

        for (int i = 0; i >= -90; i--)
        {
            transform.rotation = Quaternion.Euler(i, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            yield return null;
        }

        yield return new WaitForSeconds(3);

        for (int i = -90; i <= 0; i++)
        {
            transform.rotation = Quaternion.Euler(i, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            yield return null;
        }

        isActive = false;
    }
}
