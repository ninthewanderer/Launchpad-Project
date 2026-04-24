using System.Collections;
using UnityEngine;

public class PlatformRL : MonoBehaviour
{
    public float MoveDistance = 5f;
    public float Speed = 2f;        

    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.position;
        StartCoroutine(MoveSequenceLoop());
    }


    IEnumerator MoveSequenceLoop()
    {
        while (true) 
        {
            
            yield return StartCoroutine(MoveTo(originalPosition + Vector3.up * MoveDistance));

           
            yield return StartCoroutine(MoveTo(originalPosition));

            
            yield return StartCoroutine(MoveTo(originalPosition + Vector3.down * MoveDistance));

            
            yield return StartCoroutine(MoveTo(originalPosition));
        }
    }

   
    IEnumerator MoveTo(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, Speed * Time.deltaTime);
            yield return null;
        }
        transform.position = target;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}