﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    STANDING,
    CROUCHING,
    HIT_STARTUP,
    HIT_ACTIVE_FRAMES,
    HIT_RECOVERY,
    HITSTUN,
    BLOCKSTUN,
    HIT,
    JUMP_START,
    JUMP_LOOP,
    JUMP_LANDING,
};

public class Player : MonoBehaviour
{
    // INPUTS
    // Flèches pour se déplacer
    // A pour s'accroupir
    // l pour se prendre des dégâts, m pour infliger des dégâts
    public PlayerState State = PlayerState.STANDING;
    public float PlayerSpeed = 30.0f;
    private Rigidbody2D rb2d;
    public int PlayerHealth = 1000;
    public int ZemePoints = 0;
    private int currentPlayerHealth;
    public bool facingRight = true;
    public Animator anim;
    public Player Enemy;
    public bool PlayerControlled = true;
    public HealthBar hb;
    public ZemeBar zb;
    private bool IsAttacking = false;
    private bool IsJumping = false;
    private bool IsBlocking = false;
    public GameObject projectile;
    public bool ProjectileLaunched = false;

    void Start () {
        currentPlayerHealth = PlayerHealth;
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        // Setting des variables
        GetComponent<SpriteRenderer>().flipX = !facingRight; // Joueur regarde à gauche ou à droite ?
        GainZemePoints(0); // Set la barre de Zeme Points
    }

    // Update is called once per frame
    void FixedUpdate () {
        facingRight = transform.position.x <= Enemy.transform.position.x;
        GetComponent<SpriteRenderer>().flipX = !facingRight; // Update la rotation du joueur (gauche ou droite ?)
        if (!PlayerControlled) {
            // Si c'est pas le joueur, return directement
            return;
        }
        float inputHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        // Mouvement

        if (moveVertical < 0 && CanMove()) {
            // CROUCH
            // Lance l'animation de crouch
            anim.SetTrigger("Crouch");
        }
        if (moveVertical > 0 && CanMove() && !IsJumping) {
            // JUMP
            Debug.Log("JUMP");
            anim.SetTrigger("Jump");
            rb2d.AddForce(Vector2.up * 2100f, ForceMode2D.Impulse);
            IsJumping = true;
            rb2d.AddForce(Vector2.down * 2100f, ForceMode2D.Impulse);
        }

        if (inputHorizontal == 0.0f && moveVertical == 0.0f) {
            // Si le joueur ne veut pas bouger, on reset l'animation d'idle
            anim.SetTrigger("Idle");
        }
        if (CanMove()) {
            // rb2d.velocity = Vector2.right * inputHorizontal * PlayerSpeed;
            // rb2d.velocity = Vector2.right * inputHorizontal * 2;
            Vector3 movement = new Vector3(inputHorizontal, 0.0f, 0.0f);
            transform.Translate(movement * PlayerSpeed * Time.fixedDeltaTime);
            if (transform.position.x < -634 || transform.position.x > 570)
                transform.Translate(-movement * PlayerSpeed * Time.fixedDeltaTime);

            // rb2d.MovePosition(rb2d.position + movement * Time.fixedDeltaTime);

            if (inputHorizontal > 0.0f) {
                // Joystick à droite
                if (facingRight) {
                    anim.SetTrigger("MoveForward");
                }
                else {
                    anim.SetTrigger("MoveBackward");
                    IsBlocking = true;
                }
            }
            if (inputHorizontal < 0.0f) {
                // Joystick à gauche
                if (facingRight) {
                    anim.SetTrigger("MoveBackward");
                    IsBlocking = true;
                }
                else {
                    anim.SetTrigger("MoveForward");
                }
            }
        }
        /*
         * l -> Prends des dégâts
         * M -> Ennemi prends des dégâts
         * o -> Gagne des ZP
         * p -> Ennemi gagne des ZP
         * MAJ gauche -> bloque
         * MAJ droit -> Ennemi bloque
         * R -> Reset des attaques
         * k -> Coup de poing
         * i -> Projectile
         */
        if (Input.GetButtonDown("AttackA") && !IsAttacking) {
            // LIGHT ATTACK
            anim.SetTrigger("AttackA");
            if (Vector2.Distance(transform.position, Enemy.transform.position) < 100.0f) {
                Hit();
            }
        }
        if (Input.GetButtonDown("AttackB") && !ProjectileLaunched)
        {
            // EX MOVE - Haoken
            //GainZemePoints(-50);
            anim.SetTrigger("AttackB");
            SpawnProjectile();
        }
        if (Input.GetButton("Block"))
        {
            // MANUAL BLOCK
            IsBlocking = true;
            anim.SetTrigger("HitBlocking_Start");
        }
        else
        {
            anim.SetTrigger("HitBlocking_End");
        }

        //DEBUG
        if (Input.GetKeyDown("l")) { // Joueur prends des dégâts
            TakeDamage(25);
        }
        if (Input.GetKeyDown("m")) { // Ennemi prends des dégâts
            Enemy.TakeDamage(25);
        }
        if (Input.GetKeyDown("o")) { // Joueur gagne des Zeme Points
            GainZemePoints(10);
        }
        if (Input.GetKeyDown("p")) { // Ennemi gagne des Zeme Points
            Enemy.GainZemePoints(10);
        }
        if (Input.GetButton("BlockEnemy"))
        { // Ennemi bloque
            // MANUAL BLOCK
            Enemy.IsBlocking = true;
            Enemy.anim.SetTrigger("HitBlocking_Start");
        }
        else
        {
            Enemy.IsBlocking = false;
            Enemy.anim.SetTrigger("HitBlocking_End");
        }
        if (Input.GetKeyDown("r"))
        {
            IsAttacking = false;
            Enemy.IsAttacking = false;
        }
    }

    void Hit()
    { // Le coup a touché : cette fonction est appelée
        Enemy.TakeDamage(50);
        GainZemePoints(7);
        // On pourrait mettre les SFX ici
    }
    bool CanMove() {
        return (State == PlayerState.STANDING);
    }
    void SetState(PlayerState NewState) {
        State = NewState;
    }
    public void SetIsAttacking(bool NewIsAttacking) {
        IsAttacking = NewIsAttacking;
    }
    public bool GetIsAttacking()
    {
        return (IsAttacking);
    }

    public void TakeDamage(int amount) {
        if (IsBlocking)
        {
            amount /= 2;
        }
        currentPlayerHealth -= amount;
        hb.SetValue((float) currentPlayerHealth / (float) PlayerHealth);
        Debug.Log("DAMAGE TAKEN");
        Debug.Log("HEALTH REMAINING : " + currentPlayerHealth + " / " + PlayerHealth);
        if (currentPlayerHealth <= 0) {
            currentPlayerHealth = 0;
            Destroy(gameObject);
        } else {
            GainZemePoints(amount / 10);
        }
    }

    public void GainZemePoints(int value)
    {
        ZemePoints += value;
        if (ZemePoints < 0) {
            ZemePoints = 0;
        }
        if (ZemePoints > 100) {
            ZemePoints = 100;
        }
        if (zb) {
            zb.SetValue((float)ZemePoints / 100.0f);
        }
    }

    public bool IsAlive()
    {
        return (currentPlayerHealth > 0);
    }

    public void SpawnProjectile()
    { // Le hadoken, spawn une instance de la classe Projectile
        Vector3 pos = transform.position;
        pos.x += 50;
        pos.y += 80;
        Debug.Log("PROJECTILE SPAWNED");
        GameObject tmp = (GameObject) Instantiate(projectile, pos, transform.rotation);
        tmp.GetComponent<Projectile>().Launch(this);
        ProjectileLaunched = true;
        GainZemePoints(9);
    }

    void OnTriggerEnter(Collider col)
    {
        Debug.Log("COLLING, AHHHH");
    }
}
