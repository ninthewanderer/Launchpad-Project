using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootMovement : MonoBehaviour
{
    public enum BootType
    {
        None,
        MagnetBoots,
        SteamBoots,
        RocketBoots
    }

    public float horizontalBoostForce = 20f;
    public float verticalBoostForce = 30f;
    public float maxVerticalSpeed = 12f;

    public float maxFuel = 3f;
    public float fuelBurnRate = 1f;
    public float fuelRegenRate = 1.5f;

    public float dashCooldown = 0.5f;
    public float holdThreshold = 0.15f;
    public float dashLockoutTime = 0.15f;

    private float currentFuel;
    private float dashTimer;
    private float holdTimer;
    private float airTime;

    private bool usedHorizontalDash;
    private bool usedVerticalBoost;

    public BootType currentBoots = BootType.None;
    public PlayerController movement;

    private Rigidbody rb;

    void Start()
    {
        movement = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        currentFuel = maxFuel;
    }

    void Update()
    {
        HandleCooldowns();
        HandleFuel();

        if (currentBoots == BootType.RocketBoots)
        {
            HandleRocketBoots();
        }
    }

    void HandleRocketBoots()
    {
        if (movement == null || rb == null) return;

        if (movement.IsGrounded)
        {
            usedHorizontalDash = false;
            usedVerticalBoost = false;
            holdTimer = 0f;
            airTime = 0f;
            return;
        }
        else
        {
            airTime += Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            holdTimer = 0f;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            holdTimer += Time.deltaTime;

            if (holdTimer > holdThreshold && !usedVerticalBoost && currentFuel > 0f)
            {
                if (rb.velocity.y < maxVerticalSpeed)
                {
                    rb.AddForce(Vector3.up * verticalBoostForce, ForceMode.Acceleration);
                    currentFuel -= fuelBurnRate * Time.deltaTime;
                }

                usedVerticalBoost = true;
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (airTime > dashLockoutTime &&
                holdTimer <= holdThreshold &&
                !usedHorizontalDash &&
                dashTimer <= 0f)
            {
                usedHorizontalDash = true;
                dashTimer = dashCooldown;

                Vector3 boostDir = movement.transform.forward;
                rb.AddForce(boostDir * horizontalBoostForce, ForceMode.VelocityChange);
            }

            holdTimer = 0f;
        }
    }

    void HandleFuel()
    {
        if (movement.IsGrounded)
        {
            currentFuel += fuelRegenRate * Time.deltaTime;
            currentFuel = Mathf.Clamp(currentFuel, 0f, maxFuel);
        }
    }

    void HandleCooldowns()
    {
        if (dashTimer > 0f)
            dashTimer -= Time.deltaTime;
    }
}