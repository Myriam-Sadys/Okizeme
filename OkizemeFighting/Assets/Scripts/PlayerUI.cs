// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerUI.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Used in DemoAnimator to deal with the networked player instance UI display tha follows a given player to show its health and name
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------


using UnityEngine;
using UnityEngine.UI;

using System.Collections;
namespace Okizeme.Fight
{
    public class PlayerUI : MonoBehaviour
    {

        #region Public Properties

        [Tooltip("Pixel offset from the player target")]
        public Vector3 ScreenOffset = new Vector3(0f, 30f, 0f);

        [Tooltip("UI Text to display Player's Name")]
        public Text PlayerNameText;

        [Tooltip("UI Slider to display Player's Health")]
        public HealthBar PlayerHealthSlider;

        [Tooltip("UI Slider to display Player's Health")]
        public ZemeBar PlayerZemeSlider;

        #endregion

        #region Private Properties

        PlayerManager _target;

        float _characterControllerHeight = 0f;

        Transform _targetTransform;

        Renderer _targetRenderer;

        Vector3 _targetPosition;

        #endregion

        #region MonoBehaviour Messages

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase
        /// </summary>
        void Awake()
        {
            this.GetComponent<Transform>().SetParent(GameObject.Find("Canvas").GetComponent<Transform>());
        }

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity on every frame.
        /// update the health slider to reflect the Player's health
        /// </summary>
        void Update()
        {
            // Destroy itself if the target is null, It's a fail safe when Photon is destroying Instances of a Player over the network
            if (_target == null)
            {
                Debug.Log("DESTROYED");
                Destroy(this.gameObject);
                return;
            }


            // Reflect the Player Health
            if (PlayerHealthSlider != null)
            {
                PlayerHealthSlider.SetValue(_target.Health / 1000f);
            }

            if (PlayerZemeSlider != null)
            {
                PlayerZemeSlider.SetValue(_target.ZemePoints / 100f);
            }
        }

        /// <summary>
        /// MonoBehaviour method called after all Update functions have been called. This is useful to order script execution.
        /// In our case since we are following a moving GameObject, we need to proceed after the player was moved during a particular frame.
        /// </summary>
        void LateUpdate()
        {

            //Do not show the UI if we are not visible to the camera, thus avoid potential bugs with seeing the UI, but not the player itself.
            //if (_targetRenderer != null)
            //{
            //    Debug.Log("ah bah tu m'étonnes");
            //    this.gameObject.SetActive(_targetRenderer.isVisible);
            //}

            //# Critical
            //            Follow the Target GameObject on screen.
            //if (_targetTransform != null)
            //{
            //    _targetPosition = _targetTransform.position;
            //    _targetPosition.y += _characterControllerHeight;

            //    this.transform.position = new Vector3(0,0,0);
            //}

        }




        #endregion

        #region Public Methods

        /// <summary>
        /// Assigns a Player Target to Follow and represent.
        /// </summary>
        /// <param name="target">Target.</param>
        public void SetTarget(PlayerManager target)
        {

            if (target == null)
            {
                Debug.LogError("<Color=Red><b>Missing</b></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
                return;
            }

            // Cache references for efficiency because we are going to reuse them.
            _target = target;
            _targetTransform = _target.GetComponent<Transform>();
            _targetRenderer = _target.GetComponent<Renderer>();

            Vector3 _characterPosition = _target.transform.position;

            // Get data from the Player that won't change during the lifetime of this Component

            if (PlayerNameText != null)
            {
                PlayerNameText.text = _target.photonView.owner.NickName;
            }


            if (_targetTransform != null)
            {
                if (_characterPosition.x < 0)
                {
                    Debug.Log("Instantiating our UI if we are master I guess");
                    for (int i = 0; i < this.transform.childCount; i++)
                    {
                        _targetPosition = this.transform.GetChild(i).gameObject.transform.position;
                        _targetPosition.x = this.transform.GetChild(i).gameObject.transform.position.x * -1;
                        this.transform.GetChild(i).gameObject.transform.position = _targetPosition;
                        for (int j = 0; j < this.transform.GetChild(i).childCount; j++)
                        {
                            if (this.transform.GetChild(i).gameObject.transform.GetChild(j).gameObject.tag == "Bar")
                                this.transform.GetChild(i).gameObject.transform.GetChild(j).gameObject.GetComponent<Image>().fillOrigin = (int)Image.OriginHorizontal.Right;
                        }
                    }
                }
            }
        }

        #endregion

    }
}