using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.ParticleSystem;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleSys : MonoBehaviour
{



    // We can "scale" the curve with this value. It gets multiplied by the curve.
    public float scalar = 1.0f;

    AnimationCurve curveMin;
    AnimationCurve curveMax;
    ParticleSystem ps;
    List<ParticleCollisionEvent> collisionEvents;

    ParticleSystem.EmissionModule em;
    public float particleDmg = 0.1f;

    void Start()
    {
        ps = gameObject.GetComponent<ParticleSystem>();
        em = ps.emission;
        em.enabled = false;
        collisionEvents = new List<ParticleCollisionEvent>();
    }
    void ModifyCurve()
    {
        // Add a key to the current curve.
        // Create a "pinch" point.
        curveMin.AddKey(0.5f, 0.7f);
        curveMax.AddKey(0.5f, 0.6f);

        // Apply the changed curve.
        em.rateOverTime = new ParticleSystem.MinMaxCurve(scalar,curveMin, curveMax);
    }

    public void Activate()
    {
        
        em.enabled = true;

        // A horizontal straight line at value 1.
        curveMin = new AnimationCurve();
        curveMin.AddKey(0.0f, 1.0f);
        curveMin.AddKey(1.0f, 1.0f);

        // A horizontal straight line at value 0.5.
        curveMax = new AnimationCurve();
        curveMax.AddKey(0.0f, 0.5f);
        curveMax.AddKey(1.0f, 0.5f);


        // Apply the curve.
        em.rateOverTime = new ParticleSystem.MinMaxCurve(scalar, curveMin, curveMax);

        Invoke("ModifyCurve", 1.0f);
    }

    public void Deactivate()
    {
        em.enabled = false;

    }

    void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = ps.GetCollisionEvents(other, collisionEvents);
        int i = 0;
        while (i < numCollisionEvents)
        {
            
            if (collisionEvents[i].colliderComponent.gameObject.tag == "Enemy")
            {
                collisionEvents[i].colliderComponent.gameObject.GetComponent<EnemyStats>().TakeDamage(particleDmg);
            }
            ++i;
        }
    }
}
