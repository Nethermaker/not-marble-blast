using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script simply creates a "flash" effect every so often on the levels that use the dark storm skybox
//It also creates a large amount of lag, at least within the editor. Not sure why
public class Lightning : MonoBehaviour {

    private Light lightning;
    public Material storm;

    private void Start()
    {
        lightning = GetComponent<Light>();
        StartCoroutine("Flash");
    }

    IEnumerator Flash()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5, 25));
            for (float exposure = 1.0f; exposure >= 0f; exposure -= 0.02f)
            {
                RenderSettings.skybox.SetFloat("_Exposure", (exposure / 2f) + 1f);
                lightning.intensity = exposure;
                yield return null;
            }
        }
    }
}
