using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lives : MonoBehaviour
{
    public int livesCount = 3;
    public float damageCooldown = 1f;

    private bool canTakeDamage = true;

    void Start()
    {
        livesCount = 3;
    }

    private void OnTriggerEnter(Collider collision)
    {
        
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            LoseLife();
        }
    }

    void LoseLife()
    {
        
        if (!canTakeDamage) return;

        canTakeDamage = false;
        livesCount--;

        Debug.Log("Lives remaining: " + livesCount);

        if (livesCount <= 0)
        {
            GameOver();
        }
        else
        {
            respawn();
            StartCoroutine(DamageCooldown());
            
        }
    }

    IEnumerator DamageCooldown()
    {
        yield return new WaitForSeconds(damageCooldown);
        canTakeDamage = true;
    }
    void respawn()
    {
        // Add respawn logic
        Debug.Log("Player respawned!");
    }
    void GameOver()
    {
        Debug.Log("Game Over!");

        // UnityEngine.SceneManagement.SceneManager.LoadScene("GameOverScene");
        //Add game over logic here
    }
}