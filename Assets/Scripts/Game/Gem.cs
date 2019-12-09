using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controls the visuals relating to gems
public class Gem : MonoBehaviour {

    private void Start()
    {
        //Give each gem a random color upon level load
        MeshRenderer mr = GetComponent<MeshRenderer>();
        Color newColor = new Color(Random.value, Random.value, Random.value, 0f);
        mr.material.color = newColor;
        //Since the gems are emissive they *should* emit light, but don't, and I don' know why
        mr.material.SetColor("_EmissionColor", newColor);
    }

    private void Update()
    {
        //Spin the gem
        transform.rotation = Quaternion.Euler(new Vector3(15f, 30f, 15f) * Time.time);
    }
}
