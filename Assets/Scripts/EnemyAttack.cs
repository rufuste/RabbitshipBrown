using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Networking.Types;
using static UnityEditor.Experimental.GraphView.GraphView;
using Vector2 = UnityEngine.Vector2;

public class EnemyAttack : MonoBehaviour
{
    private float attackRange;
    private float attackDelay;
    private GameObject player;
    private PlayerStats playerStats;
    private float damage;

    private EnemyStats stats;
    float timer;


    public GameObject shot;
    public GameObject gun;
    public float fireRate;
    private float nextFire = 0;
    private int layerMask;




    // Start is called before the first frame update
    void Start()
    {
       
        // Creates reference to the EnemyStats script attached to this instance of an enemy
        stats = gameObject.GetComponent<EnemyStats>();
        damage = stats.Damage;
        attackDelay = stats.AttackDelay;

        player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();


        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (CanSeePlayer() && Time.time > nextFire)
        {
            
            nextFire = Time.time + attackDelay;

            // Finds vector between location and target
            Vector2 relativePos = player.transform.position - gun.transform.position;
            float angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;

            UnityEngine.Quaternion rotation = UnityEngine.Quaternion.AngleAxis(angle, UnityEngine.Vector3.forward);

            // Instantiates bullet on gun with rotation towards target
            Bullet bulletInstance = Instantiate(shot, gun.transform.position, rotation).GetComponent<Bullet>();
            bulletInstance.damage = damage;
            bulletInstance.source = gameObject;

        }
    }


    bool CanSeePlayer()
    {
        Vector2 rayPos = gameObject.transform.position;
        Vector2 playerPos = player.transform.position;
        Vector2 rayDir = (playerPos - rayPos).normalized;

        // Layermask for raycast, everything except enemylayer (9)
        layerMask = 1 << 9;
        layerMask = ~layerMask;
        RaycastHit2D hit = Physics2D.Raycast(rayPos, rayDir, Mathf.Infinity, layerMask);

        if (hit)
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.up) * hit.distance, Color.yellow);
            if (hit.collider.gameObject == player.gameObject)
            {
                return true;
            }
        }
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.up) * hit.distance, Color.yellow);
        return false;
    }
}
