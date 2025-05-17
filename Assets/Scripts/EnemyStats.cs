using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
public class EnemyStats : MonoBehaviour
{

    #region Sigleton
    private static EnemyStats instance;
    public static EnemyStats Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<EnemyStats>();
            }

            return instance;
        }
    }
    #endregion

    [SerializeField]
    private float health = 0;
    [SerializeField]
    private float damage = 0;
    [SerializeField]
    private float moveSpeed = 0;
    [SerializeField]
    private float spawnRate = 0;
    [SerializeField]
    private float attackSpeed = 0;
    [SerializeField]
    private float attackDelay = 0;
    [SerializeField]
    private float attackRange = 0;
    [SerializeField]
    private float maxDistanceFromPlayer = 0;
    private float maxHealth = 0;

    public float timeSinceSpawn = 0f;

    public int cost;
    public float Health { get { return health; } }
    public float Damage { get { return damage; } }
    public float SpawnRate { get { return spawnRate; } }
    public float AttackSpeed { get { return attackSpeed; } }
    public float AttackDelay { get { return attackDelay; } }
    public float AttackRange { get { return attackRange; } }
    public float TimeSinceSpawn { get { return timeSinceSpawn; } set { timeSinceSpawn = value; } }
    public float MoveSpeed { get { return moveSpeed; } }
    public float MaxDistanceFromPlayer { get { return maxDistanceFromPlayer; } }
    private bool dead;

    private GameObject score;
    public int scoreValue;
    public new AudioSource audio;
    public AudioClip hitSound;
    public Renderer rend;

    private AIPath pathFinding;
    private GameObject player;
    private IndependentBehaviour behaviour;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        rend.enabled = true;
        score = GameObject.FindGameObjectWithTag("Score");


        player = GameObject.FindGameObjectWithTag("Player");
        
        // Set up speed
        pathFinding = gameObject.GetComponent<AIPath>();
        pathFinding.maxSpeed = MoveSpeed;

        behaviour = gameObject.GetComponent<IndependentBehaviour>();
        maxHealth = health;
    }
    


    // Update is called once per frame
    void Update()
    {
        if (health <= 0 && !dead)
        {
            dead = true;
            Dead();
        }
    }

    public void Dead()
    {
        GetComponent<IndependentBehaviour>().dead();
        Score scoreScript = score.GetComponent<Score>();
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        scoreScript.IncreaseScore(scoreValue);
        rend.enabled = false;
        Destroy(gameObject, audio.clip.length);
    }

    public void TakeDamage(float damage)
    {
        audio.clip = hitSound;
        audio.pitch = UnityEngine.Random.Range(0.7f, 1f);
        audio.Play();
        health -= damage;
        
        if (health <= maxHealth / 2)
        {
            behaviour.LowHealth();
        }

    }

    public void SetTime(float time)
    {
        timeSinceSpawn = time;
    }

    internal void TakeDamage(object particleDmg)
    {
        throw new System.NotImplementedException();
    }
}
