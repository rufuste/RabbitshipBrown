using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;


public class ScrollingBackground : MonoBehaviour
{
    public float backgroundSize;

    private Transform cameraTransform;
    private Transform[] layers;
    private int downIndex;
    private int upIndex;

    public GameObject gameManager;
    PlaceObstacles placeObstacles;

    public int distance = 0;
    private int waypoint = 0;

    public GameObject grid;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        layers = new Transform[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
            layers[i] = transform.GetChild(i);

        downIndex = 0;
        upIndex = layers.Length - 1;

        placeObstacles = gameManager.GetComponent<PlaceObstacles>();

        grid = GameObject.FindGameObjectWithTag("Pathfinder");
    
    }

    private void Update()
    {
        if (cameraTransform.position.y < layers[downIndex].transform.position.y)
            ScrollDown();

        if (cameraTransform.position.y > layers[upIndex].transform.position.y)
            ScrollUp();

        
        
    }

    private void ScrollDown()
    {
        layers[upIndex].position = Vector3.up * (layers[downIndex].position.y - backgroundSize);
        downIndex = upIndex;
        upIndex--;
        if (upIndex < 0)
            upIndex = layers.Length - 1;

        // On moving backwards, subtract from distance travelled
        distance -= 1;
        //if (waypoint >= distance)
        //{
        //    waypoint = distance;
        //    // Move pathfinder grid
        //    grid.GetComponent<RefreshGrid>().Refresh();
        //}
    
    }

    private void ScrollUp()
    {
        layers[downIndex].position = Vector3.up * (layers[upIndex].position.y + backgroundSize);
        upIndex = downIndex;
        downIndex++;
        if (downIndex == layers.Length)
            downIndex = 0;

        placeObstacles.ScrollUp(upIndex, layers);

        // If length of pathfinding grid reached
        distance += 1;

        if (distance%5==0 && distance!=waypoint)
        {
            waypoint = distance;
            // Move pathfinder grid
            grid.GetComponent<RefreshGrid>().Refresh();
        }

    }


    

    

}

