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

    public BootType currentBoots = BootType.None;

    public PlayerController movement;

    void Start()
    {
        movement = GetComponent<PlayerController>();
        ApplyBoots();
    }

    public void SetBoots(BootType newBoots)
    {
        currentBoots = newBoots;
        ApplyBoots();
    }

    void ApplyBoots()
    {
        ResetAbilities();

        switch (currentBoots)
        {
            case BootType.MagnetBoots:
                ApplyMagnetBoots();
                break;

            case BootType.SteamBoots:
                ApplySteamBoots();
                break;

            case BootType.RocketBoots:
                ApplyRocketBoots();
                break;
        }
    }

    void ResetAbilities()
    {
        // Reset to default values
    }

    void ApplyMagnetBoots()
    {
        //Walk on magnetic surfaces

        //Rotate Camera to align with player orientation (Run 3 Style)
    }

    void ApplySteamBoots()
    {
        //Walk on water

        //Spray water on boss to damage
    }

    void ApplyRocketBoots()
    {
        //Hold space in air for vertical boost

        //Press space in air for horizontal boost
    }
}