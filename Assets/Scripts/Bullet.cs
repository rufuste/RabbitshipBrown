using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Bullet : MonoBehaviour
{
    public float speed;
    private Rigidbody2D rb;
    private EnemyStats enemyStats;
    private PlayerStats playerStats;
    private float playerDamage;
    private GameObject player;

    public GameObject source;
    public float lifeTime=2f;
    public float damage;

    void Awake()
    {
        Invoke("Show", 0f);
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(transform.right * speed * 10);
        
        player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();
        playerDamage = playerStats.Damage;
        
    }

    private void Show()
    {
        gameObject.SetActive(true);
        Invoke("Hide", lifeTime);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Type collided with alters result
        if (collision.collider.tag == "Enemy" && source == player)
        {
            // Fetch enemy stats
            enemyStats = collision.collider.GetComponent<EnemyStats>();
            // Deal enemy damage
            enemyStats.TakeDamage(playerDamage);
            Destroy(gameObject);

        }

        // On collide with player and bullet wasn't shot by player
        if (collision.collider.gameObject == player && source != player)
        {
            // If player alive 
            if (playerStats.Health > 0 )
            {
                // Player takes damage equal to this enemy's damage
                playerStats.TakeDamage(damage);
            }
            Destroy(gameObject);
        }

        if (collision.collider.tag == "Environment")
        {
            Destroy(gameObject);
        }

    }

}
