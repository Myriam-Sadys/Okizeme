using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Photon.PunBehaviour, IPunObservable
{
    #region Public Variables

    [Tooltip("The controller of movement of our caracter")]
    public CharacterController2D controller;

    [Tooltip("The current ZemePoints of our player")]
    public float ZemePoints = 0f;

    [Tooltip("The current Health of our player")]
    public float Health = 1000f;

    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    [Tooltip("Speed at which the player will move")]
    public float runSpeed = 40f;

    [Tooltip("The Player's UI GameObject Prefab")]
    public GameObject PlayerUiPrefab;

    public PlayerManager Enemy;
    public Transform firePoint;
    public GameObject spellPrefab;
    public bool ProjectileLaunched;

    #endregion

    #region Private Variables

    private HealthBar hb;
    private ZemeBar zb;
    private Animator animator;
    float horizontalMove = 0f;
    bool punch = false;
    bool jump = false;
    bool crouch = false;
    bool block = false;

    #endregion

    #region MonoBehaviour CallBacks

    public void Awake()
    {
        if (this.spellPrefab == null)
        {
            Debug.LogError("<Color=Red><b>Missing</b></Color> Spell Reference.", this);
        }
        else
        {
            this.spellPrefab.SetActive(false);
        }

        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instanciation when levels are synchronized
        if (photonView.isMine)
        {
            LocalPlayerInstance = gameObject;
        }

        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        animator = GetComponentInChildren<Animator>();
        if (!animator)
        {
            Debug.LogError("PlayerAnimatorManager is Missing Animator Component", this);
        }

        // Create the UI
        if (this.PlayerUiPrefab != null)
        {
            hb = PlayerUiPrefab.gameObject.transform.GetChild(1).gameObject.GetComponent<HealthBar>();
            zb = PlayerUiPrefab.gameObject.transform.GetChild(2).gameObject.GetComponent<ZemeBar>();
            GameObject _uiGo = Instantiate(this.PlayerUiPrefab) as GameObject;
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }
        else
        {
            Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
        }
    }

    void Update()
    {
        if (!animator)
        {
            return;
        }

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
                Debug.Log(Enemy.Health);
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
            GainZemePoints(15);
            animator.SetBool("IsPunching", false);
            punch = false;
        }

        if (Input.GetButtonDown("AttackB") && ProjectileLaunched == false)
        {
            if (ZemePoints >= 55)
            {
                GainZemePoints(-55);
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

        if (Input.GetButtonDown("Guard"))
        {
            TakeDamage(350);
        }
    }

    #endregion

    #region Private Methods

    private void ShootSpell()
    {
        PhotonNetwork.Instantiate(spellPrefab.name, firePoint.position, firePoint.rotation, 0);
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
        Health -= amount;
        hb.SetValue((float)Health / 1000f);
        Debug.Log("DAMAGE TAKEN");
        Debug.Log("HEALTH REMAINING : " + Health + " / " + 1000f);
        if (Health <= 0)
        {
            Health = 0;
            NetworkManagerPUN.Instance.LeaveRoom();
        }
        else
        {
            Debug.Log("ZEME GAINED : " + amount / 10);
            GainZemePoints(amount / 10);
        }
    }

    public bool IsAlive()
    {
        return (Health > 0);
    }

    void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
        jump = false;
    }

    #endregion

    #region IPunObservable implementation

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(this.ProjectileLaunched);
            stream.SendNext(this.Health);
        }
        else
        {
            // Network player, receive data
            this.ProjectileLaunched = (bool)stream.ReceiveNext();
            this.Health = (float)stream.ReceiveNext();
        }
    }

    #endregion
}
