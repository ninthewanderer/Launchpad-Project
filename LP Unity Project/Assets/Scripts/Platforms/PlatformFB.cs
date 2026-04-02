using System.Collections;
using UnityEngine;

public class PlatformFB : MonoBehaviour
{
    public float MoveDistance = 5f;
    public float Speed = 2f;

    private Vector3 originalPosition;
    private bool hasStarted = false;
    private Coroutine moveCoroutine;

    void Start()
    {
        originalPosition = transform.position;
    }

    IEnumerator MovePlatform()
    {
        Vector3 targetPosition = originalPosition + transform.forward * MoveDistance;

        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                Speed * Time.deltaTime
            );
            yield return null;
        }

        
        hasStarted = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);

            if (!hasStarted)
            {
                hasStarted = true;
                moveCoroutine = StartCoroutine(MovePlatform());
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
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
        transform.position = originalPosition;
    }
}