using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;
using static GravitySwitch;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;

public class PlayerMovement : MonoBehaviour
{

    private float jumpHeight = 10f;
    private float speed;
    private Rigidbody2D rb;
    private PlayerStats stats;
    GravitySwitch playerGravity;
    public LayerMask environmentLayer;
    public float overlapGround = 3f;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<PlayerStats>();
        speed = stats.Speed;
        playerGravity = gameObject.GetComponent<GravitySwitch>();
    }

    private void Update()
    {

        // Look at cursor
        if (playerGravity.m_GravityDirection == GravityDirection.Back)
        {
            // convert mouse position into world coordinates
            Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // get direction you want to point at
            Vector2 direction = (mouseWorldPosition - (Vector2)transform.position).normalized;

            // set vector of transform directly
            transform.up = direction;
        }
        else
        {
            transform.up = Vector3.up;
        }



        Vector3 pos = Camera.main.WorldToViewportPoint(gameObject.transform.position);
        pos.x = Mathf.Clamp01(pos.x);
        pos.y = Mathf.Clamp01(pos.y);
        gameObject.transform.position = Camera.main.ViewportToWorldPoint(pos);

    }

    void FixedUpdate()
    {
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");
        

        // if artificial gravity in place
        if (playerGravity.m_GravityDirection == GravityDirection.Down)
        {
            // Negate vertical movement
            verticalMovement = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            // Check for jump
            if (Input.GetButtonDown("Jump") && Physics2D.OverlapCircle(gameObject.transform.position, overlapGround, environmentLayer))
            {
                // Double jump
                rb.AddForce(new Vector2(0, jumpHeight), ForceMode2D.Impulse);
                // Check for jump
                if (Input.GetButtonDown("Jump"))
                {
                    rb.AddForce(new Vector2(0, jumpHeight), ForceMode2D.Impulse);
                }
            }
        }
        

        


        rb.linearVelocity = new Vector2(horizontalMovement, verticalMovement).normalized * speed;

    }

    

}
