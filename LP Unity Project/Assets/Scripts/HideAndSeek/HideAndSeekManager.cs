using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HideAndSeekManager : MonoBehaviour
{
    public GameObject pathOne;
    public GameObject pathTwo;
    public GameObject pathThree;

    private GameObject[] pathChildren;
    private String pathName;
    
    void Start()
    {
        // Picks a number between 0 and 2 which corresponds to the existing paths.
        int chosenPath = Random.Range(0, 3);
        switch (chosenPath)
        {
            // Each chosen path will hide the other paths.
            case 0:
                FindPath(pathOne);
                pathName = "Path 1";
                pathTwo.SetActive(false);
                pathThree.SetActive(false);
                break;
            case 1:
                FindPath(pathTwo);
                pathName = "Path 2";
                pathOne.SetActive(false);
                pathThree.SetActive(false);
                break;
            case 2:
                FindPath(pathThree);
                pathName = "Path 3";
                pathOne.SetActive(false);
                pathTwo.SetActive(false);
                break;
        }
    }

    // May not be necessary... finds all the children path objects in the path group.
    private void FindPath(GameObject path)
    {
        pathChildren = new GameObject[path.transform.childCount];
        for (int i = 0; i < pathChildren.Length; i++)
        {
            pathChildren[i] = path.transform.GetChild(i).gameObject;
        }
    }
    
    public GameObject[] GivePath()
    {
        return pathChildren;
    }

    public String GivePathName()
    {
        return pathName;
    }
}
