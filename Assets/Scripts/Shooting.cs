using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{

    public GameObject shot;
    public GameObject gun;
    public float fireRate;
    private float nextFire = 0;

    void Update()
    {

        if (Input.GetButton("Fire1") && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;

            // Finds vector between location and target
            Vector2 relativePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - gun.transform.position;
            float angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;

            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            // Instantiates bullet on gun with rotation towards target
            Bullet bulletInstance = Instantiate(shot, gun.transform.position, rotation).GetComponent<Bullet>();
            bulletInstance.damage = GetComponent<PlayerStats>().Damage;
            bulletInstance.source = gameObject;

        }
    }
}
