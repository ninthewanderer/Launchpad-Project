using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HideAndSeekManager : MonoBehaviour
{
    // Needs to be assigned in the Inspector.
    public GameObject pathOne;
    public GameObject pathTwo;
    public GameObject pathThree;
    public GameObject pathFour;

    public GameObject[] hidingSpots;
    
    // Internal variable which stores the chosen path.
    private String pathName;
    
    void Start()
    {
        // Picks a number between 0 and 3 which corresponds to the existing paths.
        int chosenPath = Random.Range(0, 4);
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
