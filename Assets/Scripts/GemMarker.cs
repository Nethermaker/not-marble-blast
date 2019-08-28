using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Controls the objective markers for gems
public class GemMarker : MonoBehaviour {

    public RectTransform mark;
    public Camera cam;

    private int gemAmount = 0;

    private List<GameObject> gems = new List<GameObject>();
    private List<RectTransform> markers = new List<RectTransform>();

    Plane[] planes;

    private void Start()
    {
        //Finds each gem and puts it in the gems List
        foreach (GameObject gem in GameObject.FindGameObjectsWithTag("Gem"))
        {
            gems.Add(gem);
            gemAmount++;
        }

        //Creates a marker for each gem and adds the marker to the markers List
        for (int i = 0; i < gemAmount; i++)
        {
            RectTransform marker = Instantiate(mark, transform);
            markers.Add(marker);
        }

        cam = GameObject.Find("Main Camera").GetComponent<Camera>();

    }

    private void LateUpdate()
    {
        //Not entirely sure on how planes work, but it's used to detect whether or not the camera can see any given gem
        planes = GeometryUtility.CalculateFrustumPlanes(cam);

        foreach (RectTransform marker in markers)
        {
            //Gets the gem associated with the marker
            GameObject gem = gems[markers.IndexOf(marker)];

            //Finds the point that the gem is at on the screen, and sets the marker's position to that point (and up 15 pixels)
            Vector3 screenPos = cam.WorldToScreenPoint(gem.transform.position);
            marker.transform.position = new Vector3(screenPos.x, screenPos.y + 15, screenPos.z);

            //Decides whether or not the gem is visible and sets its marker's state accordingly
            if (GeometryUtility.TestPlanesAABB(planes, gem.GetComponent<Collider>().bounds) && screenPos.z >= 1 && gem.gameObject.activeInHierarchy)
            {
                marker.gameObject.SetActive(true);
                Color gemColor = gem.GetComponent<MeshRenderer>().material.color;
                marker.GetComponent<Image>().color = new Color(gemColor.r, gemColor.g, gemColor.b, 255);
            }
            else
            {
                marker.gameObject.SetActive(false);
            }
        }
    }
}
