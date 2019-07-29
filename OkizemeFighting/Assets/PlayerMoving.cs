using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoving : MonoBehaviour
{
    public CharacterController2D controller;
    private Animator animator;
    public int PlayerHealth = 1000;
    public int ZemePoints = 0;
    private int currentPlayerHealth = 1000;
    public bool facingRight = true;
    public float runSpeed = 40f;
    float horizontalMove = 0f;
    bool punch = false;
    bool jump = false;
    bool crouch = false;
    bool block = false;
    public PlayerMoving Enemy;
    public HealthBar hb;
    public ZemeBar zb;
    public bool dontMove;
    public bool ProjectileLaunched = false;
    public Transform firePoint;
    public GameObject bulletPrefab;

    // Update is called once per frame

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        //Enemy = PhotonView.Find(1001).GetComponent<PlayerMoving>();
    }

    void Update()
    {
        if (dontMove)
            return;
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            animator.SetBool("IsJumping", true);
        }

        if (Input.GetButtonDown("AttackA") && punch == false)
        {
            animator.SetBool("IsPunching", true);
            if (Enemy)
                Debug.Log(Enemy.PlayerHealth);
            else
                Debug.Log("No enemy");
;           if (Enemy != null && Vector2.Distance(transform.position, Enemy.transform.position) < 0.7f)
            {
                Debug.Log("hitting enemy");
                Enemy.TakeDamage(50);
                GainZemePoints(10);
            }
            punch = true;
        }
        else if (Input.GetButtonUp("AttackA"))
        {
            animator.SetBool("IsPunching", false);
            punch = false;
        }

        if (Input.GetButtonDown("AttackB") && ProjectileLaunched == false)
        {
            if (ZemePoints >= 0)
            {
                GainZemePoints(-50);
                animator.SetBool("IsCastingSpell", true);
                ShootSpell();
                ProjectileLaunched = true;
            }
        }
        else if(Input.GetButtonUp("AttackB"))
        {
            animator.SetBool("IsCastingSpell", false);
            ProjectileLaunched = false;
        }

        if (Input.GetButtonDown("Guard"))
        {
            animator.SetBool("IsBlocking", true);
            block = true;
        }
        else if (Input.GetButtonUp("Guard"))
        { 
            animator.SetBool("IsBlocking", false);
            block = false;
        }

        if (Input.GetButtonDown("Crouch"))
        {
            crouch = true;
            var pos = firePoint.transform.position;
            pos.y -= 0.25f;
            firePoint.transform.position = pos;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            crouch = false;
            var pos = firePoint.transform.position;
            pos.y += 0.25f;
            firePoint.transform.position = pos;
        }
    }

    private void ShootSpell()
    {
        PhotonNetwork.Instantiate(bulletPrefab.name, firePoint.position, firePoint.rotation, 0);
    }

    public void OnCrouching(bool isCrouching)
    {
        animator.SetBool("IsCrouching", crouch);
    }

    public void OnLanding()
    {
        if (animator != null)
            animator.SetBool("IsJumping", false);
    }


    public void GainZemePoints(int value)
    {
        ZemePoints += value;
        if (ZemePoints < 0)
        {
            ZemePoints = 0;
        }
        if (ZemePoints > 100)
        {
            ZemePoints = 100;
        }
        if (zb)
        {
            zb.SetValue((float)ZemePoints / 100.0f);
        }
    }

    public void TakeDamage(int amount)
    {
        if (block)
        {
            amount /= 2;
        }
        currentPlayerHealth -= amount;
        hb.SetValue((float)currentPlayerHealth / (float)PlayerHealth);
        Debug.Log("DAMAGE TAKEN");
        Debug.Log("HEALTH REMAINING : " + currentPlayerHealth + " / " + PlayerHealth);
        if (currentPlayerHealth <= 0)
        {
            currentPlayerHealth = 0;
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            Debug.Log("ZEME GAINED : " + amount / 10);
            GainZemePoints(amount / 10);
        }
    }

    public bool IsAlive()
    {
        return (currentPlayerHealth > 0);
    }

    void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
        jump = false;
    }
}
