// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerManager.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Used in DemoAnimator to deal with the networked player instance
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.EventSystems;

namespace Okizeme.Fight
{
    public class PlayerManager : Photon.PunBehaviour, IPunObservable
    {
        #region Public Variables

        [Tooltip("The controller of movement of our caracter")]
        public CharacterController2D controller;

        [Tooltip("The Player's UI GameObject Prefab")]
        public GameObject PlayerUiPrefab;

        [Tooltip("The current ZemePoints of our player")]
        public float ZemePoints = 0f;
        private float DamageReceived;

        [Tooltip("The current Health of our player")]
        public float Health = 1f;

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        [Tooltip("Speed at which the player will move")]
        public float runSpeed = 40f;

        public Transform firePoint;
        public GameObject spellPrefab;
        public bool ProjectileLaunched;
        #endregion

        #region Private Variables

        //True, when the user is firing
        private HealthBar hb;
        private ZemeBar zb;
        private Animator animator;
        float horizontalMove = 0f;
        bool punch = false;
        bool jump = false;
        bool crouch = false;
        bool block = false;
        private bool touchingPlayer;
        private GameObject timer;
        private float timeRemaining;
        private readonly int DamagePerHit = 150;
        private readonly int DamagePerSpell = 250;
        private readonly int ZemePerDamageDone = 15;
        private readonly int ZemePerDamageReceived = 10;
        private readonly int ZemeCostSpell = 33;

        #endregion

        #region MonoBehaviour CallBacks


        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        public void Awake()
        {

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

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        public void Start()
        {
            animator = gameObject.GetComponent<Animator>();
            // Create the UI
            if (this.PlayerUiPrefab != null)
            {
                GameObject _uiGo = Instantiate(this.PlayerUiPrefab) as GameObject;
                Debug.Log("heh heh");
                _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            }
            else
            {
                Debug.LogWarning("<Color=Red><b>Missing</b></Color> PlayerUiPrefab reference on player Prefab.", this);
            }

#if UNITY_5_4_OR_NEWER
            // Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
#endif
        }


        public void OnDisable()
        {
#if UNITY_5_4_OR_NEWER
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
#endif
        }


        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity on every frame.
        /// Process Inputs if local player.
        /// Show and hide the beams
        /// Watch for end of game, when local player health is 0.
        /// </summary>
        public void Update()
        {
            // we only process Inputs and check health if we are the local player
            if (photonView.isMine)
            {
                this.ProcessInputs();
                horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
                animator.SetFloat("Speed", Mathf.Abs(horizontalMove));


                if (this.Health <= 0f)
                {
                    NetworkManagerPUN.Instance.LeaveRoom();
                }

            }
        }

        void FixedUpdate()
        {
            controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
            jump = false;
        }

        /// <summary>
        /// MonoBehaviour method called after a new level of index 'level' was loaded.
        /// We recreate the Player UI because it was destroy when we switched level.
        /// Also reposition the player if outside the current arena.
        /// </summary>
        /// <param name="level">Level index loaded</param>
        void CalledOnLevelWasLoaded(int level)
        {
            // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
            if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
            {
                transform.position = new Vector3(0f, 5f, 0f);
            }
            Debug.Log("INSTANTIATE OUR UIIIIIIIIIII YEAAAAA");
            GameObject _uiGo = Instantiate(this.PlayerUiPrefab) as GameObject;
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }

        #endregion

        #region Private Methods


#if UNITY_5_4_OR_NEWER
        void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
        {
            Debug.Log("OnSceneLoaded: " + scene.name);
            Debug.Log(loadingMode);
            this.CalledOnLevelWasLoaded(scene.buildIndex);
        }
#endif

        /// <summary>
        /// Processes the inputs. This MUST ONLY BE USED when the player has authority over this Networked GameObject (photonView.isMine == true)
        /// </summary>
        void ProcessInputs()
        {
            if (Input.GetButtonDown("Jump"))
            {
                jump = true;
                animator.SetBool("IsJumping", true);
            }

            if (Input.GetButtonDown("AttackA"))
            {
                animator.SetTrigger("AttackA");
                RaycastHit2D hit;
                Debug.DrawLine(firePoint.transform.position, transform.position + transform.right * 100, Color.red, 2.5f);
                hit = Physics2D.Raycast(firePoint.position, transform.position + transform.right * 100, Mathf.Infinity);
                if (hit.collider.name == "PlayerObject(Clone)" && hit.distance == 0)
                {
                    Debug.Log("HIT ! Found an object - distance: " + hit.distance + " name: " + hit.collider.name);
                    hit.transform.GetComponent<PlayerManager>().SendMessage("DamageEnemy", DamagePerHit);
                    PhotonView.Get(this).RPC("GainZeme", PhotonTargets.All, ZemePerDamageDone);
                }
                else
                    Debug.Log("Missed Player ! Found an object - distance: " + hit.distance + " name: " + hit.collider.name);
                punch = true;
            }
            else if (Input.GetButtonUp("AttackA"))
            {
                animator.ResetTrigger("AttackA");
                punch = false;
            }

            if (Input.GetButtonDown("AttackB") && ProjectileLaunched == false)
            {
                if (ZemePoints >= ZemeCostSpell)
                {
                    PhotonView.Get(this).RPC("GainZeme", PhotonTargets.All, -ZemeCostSpell);
                    animator.SetTrigger("AttackB");
                    GameObject clone = PhotonNetwork.Instantiate(spellPrefab.name, firePoint.transform.position, firePoint.transform.rotation, 0);
                    ProjectileLaunched = true;
                }
            }
            else if (Input.GetButtonUp("AttackB"))
            {
                animator.ResetTrigger("AttackB");
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

        private void DamageEnemy(float dmg)
        {
            PhotonView.Get(this).RPC("DealsDamage", PhotonTargets.All, dmg);
        }

        [PunRPC]
        private void DealsDamage(float Damages)
        {
            if (block)
            {
                Damages /= 2;
                Debug.Log("Damage were blocked");
            }
            this.Health -= Damages;
            GainZemePoints(ZemePerDamageReceived);
            Debug.Log("WORKED : took " + Damages + " damages and gained " + ZemePerDamageReceived + " ZEME");
        }

        [PunRPC]
        private void GainZeme(int value)
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

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {

        }

        #endregion

    }
}

//namespace Okizeme.Fight
//{
//    public class PlayerManager : Photon.PunBehaviour, IPunObservable
//    {
//        #region Public Variables

//        [Tooltip("The Player's UI GameObject Prefab")]
//        public GameObject PlayerUiPrefab;

//        [Tooltip("The controller of movement of our caracter")]
//        public CharacterController2D controller;

//        [Tooltip("The current ZemePoints of our player")]
//        public float ZemePoints = 0f;

//        [Tooltip("The current Health of our player")]
//        public float Health = 1000f;

//        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
//        public static GameObject LocalPlayerInstance;

//        [Tooltip("Speed at which the player will move")]
//        public float runSpeed = 40f;

//        public Transform firePoint;
//        public GameObject spellPrefab;
//        public bool ProjectileLaunched;

//        #endregion

//        #region Private Variables

//        private HealthBar hb;
//        private ZemeBar zb;
//        private Animator animator;
//        float horizontalMove = 0f;
//        bool punch = false;
//        bool jump = false;
//        bool crouch = false;
//        bool block = false;

//        #endregion

//        #region MonoBehaviour CallBacks

//        public void Awake()
//        {
//            if (this.spellPrefab == null)
//            {
//                Debug.LogError("<Color=Red><b>Missing</b></Color> Spell Reference.", this);
//            }
//            else
//            {
//                this.spellPrefab.SetActive(false);
//            }

//            // #Important
//            // used in NetworkManagerPUN.cs: we keep track of the localPlayer instance to prevent instanciation when levels are synchronized
//            if (photonView.isMine)
//            {
//                LocalPlayerInstance = gameObject;
//            }

//            // #Critical
//            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
//            DontDestroyOnLoad(gameObject);
//        }

//        public void Start()
//        {
//            animator = gameObject.GetComponent<Animator>();
//            if (!animator)
//            {
//                Debug.LogError("PlayerAnimatorManager is Missing Animator Component", this);
//            }

//            // Create the UI
//            if (this.PlayerUiPrefab != null)
//            {
//                GameObject _uiGo = Instantiate(this.PlayerUiPrefab) as GameObject;
//                _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
//                Debug.Log("<Color=Yellow><a>INSTANTIATING UI</a></Color>Setting UI to player");
//                hb = PlayerUiPrefab.gameObject.transform.GetChild(1).gameObject.GetComponent<HealthBar>();
//                zb = PlayerUiPrefab.gameObject.transform.GetChild(2).gameObject.GetComponent<ZemeBar>();
//            }
//            else
//            {
//                Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
//            }
//#if UNITY_5_4_OR_NEWER
//            // Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
//            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
//#endif
//        }


//        public void OnDisable()
//        {
//#if UNITY_5_4_OR_NEWER
//            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
//#endif
//        }

//        void Update()
//        {
//            if (!animator)
//            {
//                return;
//            }

//            if (photonView.isMine)
//            {
//                horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

//                animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
//                if (Input.GetButtonDown("Jump"))
//                {
//                    jump = true;
//                    animator.SetBool("IsJumping", true);
//                }

//                if (Input.GetButtonDown("AttackA") && punch == false)
//                {
//                    animator.SetBool("IsPunching", true);
//                    //if (Enemy)
//                    //    Debug.Log(Enemy.Health);
//                    //else
//                    //    Debug.Log("No enemy");
//                    //if (Enemy != null && Vector2.Distance(transform.position, Enemy.transform.position) < 0.7f)
//                    //{
//                    //    Debug.Log("hitting enemy");
//                    //    Enemy.TakeDamage(50);
//                    //    GainZemePoints(10);
//                    //}
//                    punch = true;
//                }
//                else if (Input.GetButtonUp("AttackA"))
//                {
//                    GainZemePoints(15);
//                    animator.SetBool("IsPunching", false);
//                    punch = false;
//                }

//                if (Input.GetButtonDown("AttackB") && ProjectileLaunched == false)
//                {
//                    if (ZemePoints >= 55)
//                    {
//                        GainZemePoints(-55);
//                        animator.SetBool("IsCastingSpell", true);
//                        ShootSpell();
//                        ProjectileLaunched = true;
//                    }
//                }
//                else if (Input.GetButtonUp("AttackB"))
//                {
//                    animator.SetBool("IsCastingSpell", false);
//                    ProjectileLaunched = false;
//                }

//                if (Input.GetButtonDown("Guard"))
//                {
//                    animator.SetBool("IsBlocking", true);
//                    block = true;
//                }
//                else if (Input.GetButtonUp("Guard"))
//                {
//                    animator.SetBool("IsBlocking", false);
//                    block = false;
//                }

//                if (Input.GetButtonDown("Crouch"))
//                {
//                    crouch = true;
//                    var pos = firePoint.transform.position;
//                    pos.y -= 0.25f;
//                    firePoint.transform.position = pos;
//                }
//                else if (Input.GetButtonUp("Crouch"))
//                {
//                    crouch = false;
//                    var pos = firePoint.transform.position;
//                    pos.y += 0.25f;
//                    firePoint.transform.position = pos;
//                }

//                if (Input.GetButtonDown("Guard"))
//                {
//                    TakeDamage(350);
//                }
//            }
//        }

//        public void OnCrouching(bool isCrouching)
//        {
//            animator.SetBool("IsCrouching", crouch);
//        }

//        public void OnLanding()
//        {
//            if (animator != null)
//                animator.SetBool("IsJumping", false);
//        }

//        public void GainZemePoints(int value)
//        {
//            ZemePoints += value;
//            if (ZemePoints < 0)
//            {
//                ZemePoints = 0;
//            }
//            if (ZemePoints > 100)
//            {
//                ZemePoints = 100;
//            }
//            if (zb)
//            {
//                zb.SetValue((float)ZemePoints / 100.0f);
//            }
//        }

//        public void TakeDamage(int amount)
//        {
//            if (block)
//            {
//                amount /= 2;
//            }
//            Health -= amount;
//            hb.SetValue((float)Health / 1000f);
//            Debug.Log("DAMAGE TAKEN");
//            Debug.Log("HEALTH REMAINING : " + Health + " / " + 1000f);
//            if (Health <= 0)
//            {
//                Health = 0;
//                NetworkManagerPUN.Instance.LeaveRoom();
//            }
//            else
//            {
//                Debug.Log("ZEME GAINED : " + amount / 10);
//                GainZemePoints(amount / 10);
//            }
//        }

//        public bool IsAlive()
//        {
//            return (Health > 0);
//        }

//        /// <summary>
//        /// MonoBehaviour method called when the Collider 'other' enters the trigger.
//        /// Affect Health of the Player if the collider is a beam
//        /// Note: when jumping and firing at the same, you'll find that the player's own beam intersects with itself
//        /// One could move the collider further away to prevent this or check if the beam belongs to the player.
//        /// </summary>
//        public void OnTriggerEnter(Collider other)
//        {
//            if (!photonView.isMine)
//            {
//                return;
//            }


//            // We are only interested in Beamers
//            // we should be using tags but for the sake of distribution, let's simply check by name.
//            if (!other.name.Contains("Beam"))
//            {
//                return;
//            }

//            this.Health -= 0.1f;
//        }

//        /// <summary>
//        /// MonoBehaviour method called once per frame for every Collider 'other' that is touching the trigger.
//        /// We're going to affect health while the beams are interesting the player
//        /// </summary>
//        /// <param name="other">Other.</param>
//        public void OnTriggerStay(Collider other)
//        {
//            // we dont' do anything if we are not the local player.
//            if (!photonView.isMine)
//            {
//                return;
//            }

//            // We are only interested in Beamers
//            // we should be using tags but for the sake of distribution, let's simply check by name.
//            if (!other.name.Contains("Beam"))
//            {
//                return;
//            }

//            // we slowly affect health when beam is constantly hitting us, so player has to move to prevent death.
//            this.Health -= 0.1f * Time.deltaTime;
//        }

//        void CalledOnLevelWasLoaded(int level)
//        {
//            // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
//            //if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
//            //{
//            //    transform.position = new Vector3(0f, 5f, 0f);
//            //}
//            Debug.Log("INSTANTIATE UIIIIIIIIIII YEAAAAA");
//            GameObject _uiGo = Instantiate(this.PlayerUiPrefab) as GameObject;
//            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
//        }

//        #endregion

//        #region Private Methods

//        void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
//        {
//            Debug.Log("OnSceneLoaded: " + scene.name);
//            Debug.Log(loadingMode);
//            this.CalledOnLevelWasLoaded(scene.buildIndex);
//        }

//        private void ShootSpell()
//        {
//            PhotonNetwork.Instantiate(spellPrefab.name, firePoint.position, firePoint.rotation, 0);
//        }

//        void FixedUpdate()
//        {
//            controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
//            jump = false;
//        }

//        #endregion

//        #region IPunObservable implementation

//        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
//        {
//            if (stream.isWriting)
//            {
//                // We own this player: send the others our data
//                stream.SendNext(this.ProjectileLaunched);
//                stream.SendNext(this.Health);
//            }
//            else
//            {
//                // Network player, receive data
//                this.ProjectileLaunched = (bool)stream.ReceiveNext();
//                this.Health = (float)stream.ReceiveNext();
//            }
//        }

//        #endregion
//    }
//}
