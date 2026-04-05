using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lives : MonoBehaviour
{
    public int livesCount = 3;
    public float damageCooldown = 1f;

    private bool canTakeDamage = true;

    public Image[] hearts;

    public Sprite fullHeart;
    public Sprite emptyHeart;
    private PlayerCheckpoint checkpoint;
    

    void Start()
    {
        livesCount = 3;
        checkpoint = GetComponent<PlayerCheckpoint>();
        UpdateHearts();
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

        UpdateHearts();

        if (livesCount <= 0)
        {
            GameOver();
        }
        else
        {
            respawnPlayer();
            StartCoroutine(DamageCooldown());
        }
    }

    // Added in case we want to implement healing power-ups. - Chandler
    public bool AddLife()
    {
        // Gives the player another life as long as they have less than 3 lives.
        if (livesCount < 3)
        {
            livesCount++;
            Debug.Log("Lives remaining: " + livesCount);
            UpdateHearts();
            return true;
        }
        
        Debug.Log("Lives are currently at full.");
        return false;
    }

    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < livesCount)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = emptyHeart;
        }
    }

    IEnumerator DamageCooldown()
    {
        yield return new WaitForSeconds(damageCooldown);
        canTakeDamage = true;
    }

    void respawnPlayer()
    {
        checkpoint.Respawn();
    }

    void GameOver()
    {
        Debug.Log("Game Over!");
        UnityEngine.SceneManagement.SceneManager.LoadScene("LoseScene");
    }
}