using System.Collections;
using UnityEngine;

public class PlatformFB : MonoBehaviour
{
    public float Speed = 2f;

    public Transform startPosition;
    public Transform endPosition;
    
    private Coroutine moveCoroutine;
    [System.NonSerialized] public bool hasStarted;
    [System.NonSerialized] public bool isMoving;

    public IEnumerator MovePlatform()
    {
        isMoving = true;
        while (Vector3.Distance(transform.position, endPosition.position) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                endPosition.position,
                Speed * Time.deltaTime
            );
            yield return null;
        }
        isMoving = false;
        // hasStarted = false;
    }

    public IEnumerator MovePlatformBack()
    {
       isMoving = true;
       
        while (Vector3.Distance(transform.position, startPosition.position) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, 
                startPosition.position, 
                Speed * Time.deltaTime);
            yield return null;
        }

        isMoving = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.SetParent(gameObject.transform);
            if (!hasStarted)
            {
                hasStarted = true;
                // moveCoroutine = StartCoroutine(MovePlatform());
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.SetParent(gameObject.transform);
            if (!hasStarted)
            {
                hasStarted = true;
                // moveCoroutine = StartCoroutine(MovePlatform());
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.SetParent(null);
        }
    }

    
    public void ResetMovement()
    {
        
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
       
        hasStarted = false;
        transform.position = startPosition.position;
    }
}