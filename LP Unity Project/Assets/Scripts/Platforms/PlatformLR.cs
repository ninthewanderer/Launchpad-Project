using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformLR : MonoBehaviour
{
    public float LeftBound = -5f;
    public float RightBound = 5f;
    public float Speed = 2f;
    private int direction = 1;

    void Update()
    {
        
        transform.Translate(Vector3.right * direction * Speed * Time.deltaTime);
       
        if (transform.position.x >= RightBound)
        {
            direction = -1; 
        }
        else if (transform.position.x <= LeftBound)
        {
            direction = 1; 
        }

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
