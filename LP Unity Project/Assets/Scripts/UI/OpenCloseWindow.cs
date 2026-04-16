using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

public class OpenCloseWindow : MonoBehaviour
{
    [Header("Window Setup")] 
    [SerializeField, RequiredMember] private GameObject window;
    [SerializeField, RequiredMember] private RectTransform windowRectTransform;
    [SerializeField, RequiredMember] private CanvasGroup windowCanvasGroup;

    public enum AnimateToDirection
    {
        Top,
        Bottom,
        Left,
        Right
    }
    
    [Header("Animation Setup")]
    [SerializeField] private AnimateToDirection openDirection = AnimateToDirection.Top;
    [SerializeField] private AnimateToDirection closeDirection = AnimateToDirection.Bottom;
    [Space] 
    [SerializeField] private Vector2 distanceToAnimate = new Vector2(100, 100);
    [SerializeField] private AnimationCurve easingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [Range(0, 1f)] [SerializeField] private float animationDuration = 0.5f;

    private bool _isOpen;
    private Vector2 _initialPosition;
    private Vector2 _currentPosition;
    
    private Vector2 _upOffset;
    private Vector2 _downOffset;
    private Vector2 _leftOffset;
    private Vector2 _rightOffset;
    
    private Coroutine _animateWindowCoroutine;
    
    private void OnValidate()
    {
        if (window != null)
        {
            windowRectTransform = window.GetComponent<RectTransform>();
            windowCanvasGroup = window.GetComponent<CanvasGroup>();
        }
        
        distanceToAnimate.x = Mathf.Max(0, distanceToAnimate.x);
        distanceToAnimate.y = Mathf.Max(0, distanceToAnimate.y);
    }
}
