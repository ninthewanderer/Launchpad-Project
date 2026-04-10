using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetectionBootsUI : MonoBehaviour
{
    public RectTransform chargeBar;
    public float chargeBarWidth, chargeBarHeight;
    public float maxCharge;
    private float currentCharge;

    public void SetMaxCharge(float maximum)
    {
        maxCharge = maximum;
    }

    public void SetCurrentCharge(float currCharge)
    {
        currentCharge = currCharge;
        float newWidth = (currentCharge / maxCharge) * chargeBarWidth;
        
        chargeBar.sizeDelta = new Vector2(newWidth, chargeBarHeight);
    }
}
