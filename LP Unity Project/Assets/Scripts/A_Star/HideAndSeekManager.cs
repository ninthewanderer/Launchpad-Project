using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideAndSeekManager : MonoBehaviour
{
    public GameObject pathOne;
    public GameObject pathTwo;
    public GameObject pathThree;

    public GameObject[] pathChildren;
    
    // Start is called before the first frame update
    void Start()
    {
        int chosenPath = Random.Range(0, 3);
        switch (chosenPath)
        {
            case 0:
                FindPath(pathOne);
                break;
            case 1:
                FindPath(pathTwo);
                break;
            case 2:
                FindPath(pathThree);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FindPath(GameObject path)
    {
        pathChildren = new GameObject[path.transform.childCount];
        foreach (Transform child in path.transform)
    }
}
