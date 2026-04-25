using System.Collections;
using UnityEngine;

public class PlatformRL : MonoBehaviour
{
    public float MoveDistance = 5f;
    public float Speed = 2f;
    [SerializeField] private Vector3 moveDirection;

    private Vector3 originalPosition;

    void Start()
    {
        
    }

    private void OnEnable()
    {
        originalPosition = transform.position;
        Debug.Log("original position: " + originalPosition);    
        StartCoroutine(MovePlatform());
    }



    public IEnumerator MovePlatform()
    {
        Vector3 targetPosition = originalPosition + moveDirection;
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                Speed * Time.deltaTime
            );
            yield return null;
        }
        yield return new WaitForSeconds(2f);

        StartCoroutine(MovePlatformBack());
        // hasStarted = false;
    }

    public IEnumerator MovePlatformBack()
    {

        while (Vector3.Distance(transform.position, originalPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                originalPosition,
                Speed * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(2f);
        StartCoroutine(MovePlatform());

    }
}