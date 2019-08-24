using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{

    public float speed = 20f;
    public int damage = 40;
    public Rigidbody2D rb;
    public GameObject impactEffect;

    // Use this for initialization
    void Start()
    {
        rb.velocity = transform.right * speed;
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        PlayerMoving enemy = hitInfo.GetComponent<PlayerMoving>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
        GameObject shot;

        shot = PhotonNetwork.Instantiate(impactEffect.name, transform.position, transform.rotation, 0);

        PhotonNetwork.Destroy(gameObject);
        //Destroy(shot);
    }

}
