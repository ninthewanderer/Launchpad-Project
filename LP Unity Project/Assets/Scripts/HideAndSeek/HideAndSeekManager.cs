using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;

public class HideAndSeekManager : MonoBehaviour
{
    private GameObject[] paths;
    private GameObject[] hidingSpots;
    private String pathName;
    private int chosenPath;
    
    void Start()
    {
        // Locates and orders all paths in the scene.
        paths = GameObject.FindGameObjectsWithTag("Path");
        var orderedPaths = paths.OrderBy(path => path.name);
        paths = orderedPaths.ToArray();
        
        // Locates and orders all hiding spots in the scene.
        hidingSpots = GameObject.FindGameObjectsWithTag("HidingSpot");
        var orderedSpots = hidingSpots.OrderBy(hidingSpot => hidingSpot.name);
        hidingSpots = orderedSpots.ToArray();
        
        // Picks a number between 0 and the total number of paths which corresponds to the existing paths.
        chosenPath = Random.Range(0, paths.Length);
        
        // Each chosen path will hide the other paths & hiding spots.
        for (int i = 0; i < paths.Length; i++)
        {
            if (i != chosenPath)
            {
                paths[i].SetActive(false);
            }
        }
        
        for (int i = 0; i < hidingSpots.Length; i++)
        {
            if (i != chosenPath)
            {
                hidingSpots[i].SetActive(false);
            }
        }
    }

    // Returns the currently active hiding spot chosen by the H&S Manager.
    public GameObject GetHidingSpot()
    {
        return hidingSpots[chosenPath];
    }
}
