using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DetectionBoots : MonoBehaviour
{
    public HideAndSeekManager hnsManager;
    public float detectionRadius;
    public LayerMask catTraces;
    
    private GameObject player;
    private GameObject[] randomPath; // may not need.
    private String pathName;
    
    
    void Start()
    {
        // Obtains the path picked by the Hide & Seek Manager.
        randomPath = hnsManager.GivePath(); // may not need.
        pathName = hnsManager.GivePathName();
    }
    
    void Update()
    {
        // E on keyboard, Y on controller
        // This activates the detection boots ability.
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Joystick1Button3))
        {
            Debug.Log("Checking for traces.");
            CheckForTraces();
        }
    }

    private void CheckForTraces()
    {
        Collider[] traces = Physics.OverlapSphere(transform.position, detectionRadius, catTraces, QueryTriggerInteraction.Collide);

        if (traces.Length != 0)
        {
            foreach (Collider trace in traces)
            {
                GameObject traceObj = trace.transform.gameObject;
                if (traceObj.CompareTag(pathName))
                {
                    int defaultLayer = LayerMask.NameToLayer("Default");
                    traceObj.layer = defaultLayer;
                    
                    // FIXME: revert it back to its previous layer after a set amount of time. (coroutine)
                    // ensure player has detection boot cooldown
                    Debug.Log("Trace detected!");
                }
            }
        }
    }
}
