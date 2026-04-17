using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DetectionBoots : MonoBehaviour
{
    [Header("------- Detection Boots -------")]
    public LayerMask traceLayer;
    public float detectionRadius;
    public float disappearTime;
    public float abilityCooldown;
    private bool onCooldown = false;
    public DetectionBootsUI chargeBar;
    public float maxCharge;
    private float currentCharge;
    public float chargeLost;
    
    void Start()
    {
        // Starts the player off with full charge.
        chargeBar.SetMaxCharge(maxCharge);
        chargeBar.SetCurrentCharge(maxCharge);
        currentCharge = maxCharge;
        chargeBar.BarOffCooldown();
        StartCoroutine(CooldownCheck()); // Starts constant boot cooldown management.
    }

    void Update()
    {
        // E on keyboard, Y on controller
        // This activates the detection boots ability.
        if (!onCooldown && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Joystick1Button3)))
        {
            onCooldown = true;
            SetCharge(-chargeLost);
            CheckForTraces();
            GetComponent<PlayerSounds>()?.PlayDetectionBootsSound();
        }
    }

    // Checks for traces of the cat near the player.
    private void CheckForTraces()
    {
        Collider[] traces = Physics.OverlapSphere(transform.position, detectionRadius, traceLayer,
            QueryTriggerInteraction.Collide);

        if (traces.Length != 0)
        {
            foreach (Collider trace in traces)
            {
                GameObject traceObj = trace.transform.gameObject;
                if (traceObj.CompareTag("Trace") && traceObj.activeSelf)
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
        int traceLayerInt = LayerMask.NameToLayer("Cat_Traces");
        traceObj.layer = traceLayerInt;
    }

    // Manages the ability cooldown for the boots.
    private IEnumerator CooldownCheck()
    {
        while (true)
        {
            if (onCooldown)
            {
                chargeBar.BarOnCooldown();
                yield return new WaitForSeconds(abilityCooldown);
                chargeBar.BarOffCooldown();
                onCooldown = false;
            }
            yield return null;
        }
    }

    // Manages the charge bar on the top right.
    private void SetCharge(float newCharge)
    {
        // Adds the newCharge value onto currentCharge, ensuring it never drops below 0 or goes above maxCharge.
        currentCharge += newCharge;
        currentCharge = Mathf.Clamp(currentCharge, 0, maxCharge);

        // If the player runs out of charge, they lose.
        if (currentCharge == 0)
        {
            SceneManager.LoadScene("LoseScene");
        }
        chargeBar.SetCurrentCharge(currentCharge);
    }
}