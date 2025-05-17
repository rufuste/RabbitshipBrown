using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Numerics;
using UnityEditor.Animations;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class CommandEnemies : MonoBehaviour
{
    public float summonPoints;

    public GameObject enemyLine;
    public GameObject enemyRabbit;
    public GameObject currentCaptain;

    public float accrualRate = 2f;
    public int squadSize = 5;
    private int spawned = 0;

    private int countProgress = 0;
    public Score playerScore;
    public float difficultyIncrease = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        playerScore = GameObject.FindGameObjectWithTag("Score").GetComponent<Score>();
        InvokeRepeating("AccruePoints", 3, accrualRate);

    }

    // Update is called once per frame
    void Update()
    {
        // Increase spawn rate as player score increases
        if (playerScore.GetScore() > countProgress && playerScore.GetScore() % 5 == 0)
        { 
            countProgress=playerScore.GetScore();
            accrualRate -= difficultyIncrease;
        }

        // Summon rabbits according to number of summon points available
        if (summonPoints>0)
        {
            // Spawn location bounds
            Vector3 min = enemyLine.GetComponent<MeshFilter>().mesh.bounds.min;
            Vector3 max = enemyLine.GetComponent<MeshFilter>().mesh.bounds.max;

            Vector3 spawn = enemyLine.transform.position - new Vector3(Random.Range(min.x * 5, max.x * 5), 0);



            IndependentBehaviour.ranks _role;

            GameObject rabbitInstance = Instantiate(enemyRabbit, spawn, Quaternion.identity);

            // If captain died or squad full
            if (currentCaptain == null || !currentCaptain || spawned % squadSize == 0)
            {
                // New rabbit is a captain
                _role = IndependentBehaviour.ranks.Captain;
                currentCaptain = rabbitInstance;
                spawned = 0;

            }
            else
            {
                // Else set to follow current captain
                _role = IndependentBehaviour.ranks.Private;
            }



            summonPoints -= enemyRabbit.GetComponent<EnemyStats>().cost;

            IndependentBehaviour behaviour = rabbitInstance.GetComponent<IndependentBehaviour>();
            
            // Set behaviour of instance
            behaviour.role = _role;
            if (currentCaptain)
            {
                behaviour.superior = currentCaptain;
            }

            if (behaviour && behaviour.superior != null && rabbitInstance!=null)
            {
                if (_role == IndependentBehaviour.ranks.Private)
                    behaviour.superior.GetComponent<IndependentBehaviour>().squad.Add(rabbitInstance);
                else
                    behaviour.squad.Add(rabbitInstance);
            }

            spawned++;
            
        }
    }

    void AccruePoints()
    {
        summonPoints++;
        summonPoints++;
        summonPoints++;
        summonPoints++;
        summonPoints++;

        // Randomness
        accrualRate = Random.Range(accrualRate / 2, accrualRate * 2) ;
    }


}
