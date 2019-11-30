using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Okizeme.Fight
{
    public class BulletScript : Photon.PunBehaviour
    {

        public float speed = 20f;
        private readonly int DamagePerSpell = 250;
        public Rigidbody2D rb;
        public GameObject impactEffect;
        private bool alreadyHit = false;
        private float timer = 0f;
        private RaycastHit2D hit;

        // Use this for initialization
        void Start()
        {
            rb.velocity = transform.right * speed;
            Destroy(gameObject, 2f);//Shoot();
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= 1f)
            {
                alreadyHit = false;
            }
        }

        void OnTriggerEnter2D(Collider2D hitInfo)
        {
            if (alreadyHit)
                return;

            PlayerManager enemy = hitInfo.GetComponent<PlayerManager>();
            if (this.photonView.isMine && enemy != null)
            {
                alreadyHit = true;
                timer = 0f;
                Debug.Log("Is an ennemy");
                enemy.SendMessage("DamageEnemy", DamagePerSpell);
            }
            else
                Debug.Log("Wall or not my bullet");
            //GameObject shot = Instantiate(this.impactEffect, transform.position, transform.rotation);
            if (this.photonView.isMine)
            {
                Debug.Log("It's my bullet");
                GameObject clone = PhotonNetwork.Instantiate(impactEffect.name, transform.position, transform.rotation, 0);
                PhotonNetwork.Destroy(gameObject);
            }
            else
            {
                Debug.Log("It's enemy's bullet");
                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<CircleCollider2D>().enabled = false;
            }

        }

    }
}

