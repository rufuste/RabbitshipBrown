using JetBrains.Annotations;
using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Transactions;
using UnityEditor.Experimental.Rendering;
using UnityEditor.Tilemaps;
using UnityEditor.UI;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;

public class IndependentBehaviour : MonoBehaviour
{

    private AIDestinationSetter destination;
    private GameObject player;
    private AIPath ai;
    private int layerMask;

    private int squadSize;
    public List<GameObject> squad;
    public GameObject superior;
    public IndependentBehaviour superiorBehaviour;
    private CommandEnemies commander;

    public float coverPaddingX;
    public float coverPaddingY;
    public float coverPaddingVariance;
    public float coverTime = 5;
    private bool deadSuperiorTriggered = false;
    private int deadTeammates = 0;

    public Transform target;
    public Vector2 distanceLimit = new Vector2(5.0f, 5.0f);

    public enum ranks { Private, Captain};
    public ranks role;

    [Range(0.0f, 20.0f)] public float minSpeed;
    [Range(0.0f, 20.0f)] public float maxSpeed;
    [Range(0.0f, 10.0f)] public float neighbourDistance;
    [Range(1.0f, 20.0f)] public float rotationSpeed;


    private float speed;
    public Vector3 goalPos = Vector3.zero;
    public float avoidDistance;

    Rigidbody2D rb2d;
    public float closenessToLeader= 1f;


    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();

        commander = GameObject.FindGameObjectWithTag("Commander").GetComponent<CommandEnemies>();
        player = GameObject.FindGameObjectWithTag("Player");
        destination = gameObject.GetComponent<AIDestinationSetter>();
        ai = gameObject.GetComponent<AIPath>();
        superior = commander.currentCaptain;
        superiorBehaviour = superior.GetComponent<IndependentBehaviour>();

        squadSize = commander.squadSize;
        squad = new List<GameObject>();

        if (role == ranks.Captain)
        {    
            goalPos = transform.position;
            
            // Initialise pathfinding destination to player
            HuntForPlayer();
        }
        

        if (role == ranks.Private)
        {
            // speed = Random.Range(superiorBehaviour.minSpeed, superiorBehaviour.maxSpeed);
            target = superior.transform;

            destination.target = target;
            ai.SearchPath();
        }

    }

    private void Update()
    {
        if (role == ranks.Captain)
        {
            goalPos = transform.position;
        }

        // If Superior dead, hunt player
        if (role == ranks.Private && (!superior || superior.gameObject == null)&& !deadSuperiorTriggered)
        {
            deadSuperiorTriggered = true;
            HuntForPlayer();
        }

        // If Superior alive, follow
        if (role == ranks.Private && superior != null)
        {
            Flock();
        }

        
    }

    
    
    void Flock()
    {
        Bounds b = new Bounds(superior.transform.position, superiorBehaviour.distanceLimit * 2.0f);
        // If captain out of bounds, move towards
        bool turning = !b.Contains(transform.position);

        if (turning)
        {
            target = superior.transform;
            if (destination.target != target)
            {
                destination.target = target;
            }
        }

        else
        {

            if (Random.Range(0, 100) < 10)
            {
                ai.maxSpeed = Random.Range(superiorBehaviour.minSpeed, superiorBehaviour.maxSpeed);
            }

            // Apply Flocking rules
            if (Random.Range(0, 100) < 10)
            {
               ApplyRules();
            }

            
            ai.endReachedDistance = closenessToLeader;
        }
        //transform.Translate(0.0f, speed * Time.deltaTime, 0.0f);

    }
    private void ApplyRules()
    {

        GameObject[] gos;

        gos = superiorBehaviour.squad.ToArray();
        Vector3 vCentre = Vector3.zero;
        Vector3 vAvoid = Vector3.zero;

        float gSpeed = 0.01f;
        float mDistance;
        int groupSize = 0;


        // Find direction to fit flock
        for (int i = 0; i < gos.Length; i++)
        {
            GameObject go = gos[i];
            if (go != null && go != gameObject )
            {

                mDistance = Vector3.Distance(go.transform.position, transform.position);
                if (mDistance <= superiorBehaviour.neighbourDistance)
                {

                    vCentre += go.transform.position;
                    groupSize++;

                    if (mDistance < 1.0f)
                    {

                        vAvoid = vAvoid + (transform.position - go.transform.position);
                    }

                }
            }
        }

        if (groupSize > 0)
        {

            vCentre = vCentre / groupSize + (superiorBehaviour.goalPos - transform.position);
            speed = gSpeed / groupSize;

            if (speed > superiorBehaviour.maxSpeed)
            {
                ai.maxSpeed = superiorBehaviour.maxSpeed;
            }

            Vector3 direction = (vCentre + vAvoid) - transform.position;
            // turning
            if (direction != Vector3.zero)
            {
                GameObject directionO = new GameObject();
                
                directionO.transform.position = direction*avoidDistance;

                rb2d.AddForce(direction*avoidDistance);
            
                //target = directionO.transform;
                //goToDestination(target);
                //target = superior.transform;
                //destination.target = target;
                //ai.SearchPath();

            }
        }
    }

    IEnumerator goToDestination(Transform target) 
    {
        
        destination.target = target;
        ai.SearchPath();
        while (!ai.reachedDestination)
        {
            yield return null;
        }

    }
    public void dead()
    {
        if (role == ranks.Captain)
        {
            int i = 0;
            GameObject promote = gameObject;
            foreach (GameObject priv in squad)
            {

                if (priv && priv!=null)
                {
                    i++;
                    if (i == 1)
                    {
                        promote = priv;
                        squad.Remove(gameObject);
                        priv.GetComponent<IndependentBehaviour>().squad = squad;
                        break;
                    }
                    priv.GetComponent<IndependentBehaviour>().superior = promote;
                }
            }
   
        }
        else
        {
            if (superior)
                superior.GetComponent<IndependentBehaviour>().squad.Remove(gameObject);
        }
    }
    void HuntForPlayer()
    {
        target = player.transform;
        destination.target = target;
    }

    public void LowHealth()
    {
        // Character took damage
        TakeCover();
        if (role == ranks.Private && superior)
        { superiorBehaviour.lowTeammate(); }
    }

    public void lowTeammate()
    {
        deadTeammates++;
        if (deadTeammates > (squad.Count() % 2))
        {
            TakeCover();
        }

    }

    void TakeCover()
    {

        GameObject[] obstacleLocations;
        obstacleLocations = GameObject.FindGameObjectsWithTag("Obstacle");
        Vector3[] coverLocations = new Vector3[obstacleLocations.Length];


        // For each obstacle
        for (int i = 0; i < obstacleLocations.Length; i++)
        {

            coverPaddingX += UnityEngine.Random.Range(-coverPaddingVariance, coverPaddingVariance);
            Vector3 cover = new Vector3(obstacleLocations[i].transform.position.x + coverPaddingX, obstacleLocations[i].transform.position.y + coverPaddingY, obstacleLocations[i].transform.position.z);


            // Everything except player
            layerMask = 1 << 7;
            layerMask = ~layerMask;

            // Direction of object from player

            Vector2 rayPos = player.transform.position;
            Vector2 rayDir = (transform.position - player.transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(rayPos, rayDir, Mathf.Infinity, layerMask); 
            
            // Draw raycast from player, if this character hit
            if (hit && hit.collider.gameObject == gameObject)
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.up) * hit.distance, Color.yellow);
                // Bad Cover
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.up) * hit.distance, Color.yellow);
                // Add to potential covers
                coverLocations[i] = cover;
            }
        }


        // Find closest
        Vector3 closest = Vector3.zero;
        float closestMeasure = 0;
        float measure = 0;
        for (int i = 0; i < coverLocations.Length; ++i)
        {
            measure = Vector3.Distance(coverLocations[i], gameObject.transform.position);
            if (closestMeasure == 0 || measure < closestMeasure)
            {
                closestMeasure = measure;
                closest = coverLocations[i];
            }
        }

        // Create point for character to move to
        GameObject coverPoint = new GameObject();
        Transform newTransform = coverPoint.transform;
        coverPoint.transform.position = closest;
        if (closestMeasure > 0)
        {
            target = newTransform.transform;
            destination.target = target;
            ai.SearchPath();
            StartCoroutine(Hiding());

        }

    }

    IEnumerator Hiding()
    {
       
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(coverTime);
        HuntForPlayer();

    }



}
