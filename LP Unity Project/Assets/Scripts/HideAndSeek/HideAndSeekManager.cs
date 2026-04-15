using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HideAndSeekManager : MonoBehaviour
{
    /* FIXME: make this script more modular so that it can be utilized in the tutorial.
     - Paths must now be in a GameObject array defined in the Inspector; remove assignable individual paths
     - Start() must operate on the logic that we don't know how many paths currently exist */
    
    // Needs to be assigned in the Inspector.
    public GameObject[] paths;
    
    public GameObject pathOne;
    public GameObject pathTwo;
    public GameObject pathThree;
    public GameObject pathFour;

    public GameObject[] hidingSpots;
    
    // Internal variable which stores the chosen path.
    private String pathName;
    
    void Start()
    {
        // Picks a number between 0 and the total number of paths which corresponds to the existing paths.
        int chosenPath = Random.Range(0, paths.Length);
        switch (chosenPath)
        {
            // Each chosen path will hide the other paths & hiding spots.
            case 0:
                pathName = "Path 1";
                pathTwo.SetActive(false);
                pathThree.SetActive(false);
                pathFour.SetActive(false);

                for (int i = 0; i < hidingSpots.Length; i++)
                {
                    if (i != chosenPath)
                    {
                        hidingSpots[i].SetActive(false);
                    }
                }
                break;
            
            case 1:
                pathName = "Path 2";
                pathOne.SetActive(false);
                pathThree.SetActive(false);
                pathFour.SetActive(false);
                
                for (int i = 0; i < hidingSpots.Length; i++)
                {
                    if (i != chosenPath)
                    {
                        hidingSpots[i].SetActive(false);
                    }
                }
                break;
            
            case 2:
                pathName = "Path 3";
                pathOne.SetActive(false);
                pathTwo.SetActive(false);
                pathFour.SetActive(false);
                
                for (int i = 0; i < hidingSpots.Length; i++)
                {
                    if (i != chosenPath)
                    {
                        hidingSpots[i].SetActive(false);
                    }
                }
                break;
            
            case 3:
                pathName = "Path 4";
                pathOne.SetActive(false);
                pathTwo.SetActive(false);
                pathThree.SetActive(false);
                
                for (int i = 0; i < hidingSpots.Length; i++)
                {
                    if (i != chosenPath)
                    {
                        hidingSpots[i].SetActive(false);
                    }
                }
                break;
        }
    }

    // Returns the name of the path that has been randomly chosen.
    public String GivePathName()
    {
        return pathName;
    }

    // Returns the currently active hiding spot chosen by the H&S Manager.
    public GameObject GetHidingSpot()
    {
        foreach (GameObject spot in hidingSpots)
        {
            if (spot.activeInHierarchy)
                return spot;
        }
        return null;
    }
}
