using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionBoots : MonoBehaviour
{
    public HideAndSeekManager hnsManager;
    public LayerMask catTraces;
    public float detectionRadius;
    public float disappearTime;
    public float abilityCooldown;

    private String pathName;
    private bool onCooldown = false;

    void Start()
    {
        // Obtains the name of the path picked by the Hide & Seek Manager.
        pathName = hnsManager.GivePathName();
        StartCoroutine(CooldownCheck());
    }

    void Update()
    {
        // E on keyboard, Y on controller
        // This activates the detection boots ability.
        if (!onCooldown && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Joystick1Button3)))
        {
            onCooldown = true;
            CheckForTraces();
        }
        else if (onCooldown && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Joystick1Button3)))
        {
            Debug.Log("Ability is on cooldown. Please wait " + abilityCooldown + " seconds.");
        }
    }

    // Checks for traces of the cat near the player.
    private void CheckForTraces()
    {
        Collider[] traces = Physics.OverlapSphere(transform.position, detectionRadius, catTraces,
            QueryTriggerInteraction.Collide);

        if (traces.Length != 0)
        {
            foreach (Collider trace in traces)
            {
                GameObject traceObj = trace.transform.gameObject;
                if (traceObj.CompareTag(pathName))
                {
                    int defaultLayer = LayerMask.NameToLayer("Default");
                    traceObj.layer = defaultLayer;
                    StartCoroutine(TraceDisappear(traceObj));
                }
            }
        }
    }

    // Makes the traces of the cat disappear after a specified amount of time.
    private IEnumerator TraceDisappear(GameObject traceObj)
    {
        yield return new WaitForSeconds(disappearTime);
        int traceLayer = LayerMask.NameToLayer("Cat_Traces");
        traceObj.layer = traceLayer;
    }

    // Manages the ability cooldown for the boots.
    private IEnumerator CooldownCheck()
    {
        while (true)
        {
            if (onCooldown)
            {
                yield return new WaitForSeconds(abilityCooldown);
                Debug.Log("Ability can now be used.");
                onCooldown = false;
            }
            yield return null;
        }
    }
}