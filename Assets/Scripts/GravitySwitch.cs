using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySwitch : MonoBehaviour
{
    public enum GravityDirection { Down, Back };
    public GravityDirection m_GravityDirection;
    Rigidbody2D rb;
    public float maxGravity =  20;

    void Start()
    {
        m_GravityDirection = GravityDirection.Back;
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        switch (m_GravityDirection)
        {

            // Default to top down
            case GravityDirection.Back:

                
                if (Input.GetButtonDown("Space"))
                {
                    // Turn on gravity
                    m_GravityDirection = GravityDirection.Down;
                    Physics2D.gravity = new Vector2(0, -9.8f);
                    rb.gravityScale = maxGravity;
                    
                }
                break;
            
            case GravityDirection.Down:
                
                
                //Press the space key to switch to the left direction
                if (Input.GetButtonDown("Space"))
                {                    
                    m_GravityDirection = GravityDirection.Back;
                    Physics2D.gravity = new Vector2(0, 0f);
                    rb.gravityScale = 0.0f;
                    rb.constraints = RigidbodyConstraints2D.None;
                }
                break;

            
        }
    }
}