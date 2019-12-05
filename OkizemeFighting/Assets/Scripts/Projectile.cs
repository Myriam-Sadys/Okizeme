using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Okizeme.Fight
{
    public class Projectile : MonoBehaviour
    {

        public float Speed = 1000.0f;
        public float Durability = 1;
        public int Damage = 1;
        public PlayerManager Launcher;
        public bool facingRight;
        public Rigidbody2D rb;

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 tmp = transform.position;
            if (facingRight)
            {
                transform.position += transform.right * Time.deltaTime * Speed;
            }
            else
            {
                transform.position -= transform.right * Time.deltaTime * Speed;
            }
            if (IsColliding(Launcher.GetComponent<BoxCollider2D>()))
            {
                Launcher.GainZemePoints(6);
                Launcher.ProjectileLaunched = false;
                Destroy(gameObject);
            }
        }

        public void Launch(PlayerManager player)
        {
            Launcher = player;
            if (Launcher)
            {
                //facingRight = Launcher.facingRight;
                /*rb.velocity = transform.forward * Speed;
                if (!facingRight) {
                    rb.velocity *= (-1);
                }*/
                GetComponent<SpriteRenderer>().flipX = !facingRight;
                GetComponent<SpriteRenderer>().enabled = true;
            }
        }

        void OnTriggerEnter(Collider col)
        {
            Debug.Log("COLLING, AHHHH");
        }

        bool IsColliding(BoxCollider2D enemy_collider)
        {
            return (GetComponent<BoxCollider2D>().bounds.Intersects(enemy_collider.bounds));
        }
    }
}