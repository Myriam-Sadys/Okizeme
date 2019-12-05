using UnityEngine;
using UnityEngine.EventSystems;

using System.Collections;
/// <summary>
/// Player manager.
/// Handles fire Input and Beams.
/// </summary>
/// 
public class PlayerAnimator : MonoBehaviour
{
    #region Private Fields

    [Tooltip("The Beams GameObject to control")]
    [SerializeField]
    private GameObject beams;
    //True, when the user is firing
    bool IsFiring;
    #endregion

    #region MonoBehaviour CallBacks

    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
    /// </summary>
    void Awake()
    {
        if (beams == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> Beams Reference.", this);
        }
        else
        {
            beams.SetActive(false);
        }
    }

    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity on every frame.
    /// </summary>
    void Update()
    {

        // we only process Inputs if we are the local player
        //if (photonView.IsMine)
        //{
        //    ProcessInputs();
        //}

        // trigger Beams active state
        if (beams != null && IsFiring != beams.activeSelf)
        {
            beams.SetActive(IsFiring);
        }
    }

    #endregion

    #region Custom

    /// <summary>
    /// Processes the inputs. Maintain a flag representing when the user is pressing Fire.
    /// </summary>
    void ProcessInputs()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (!IsFiring)
            {
                IsFiring = true;
            }
        }
        if (Input.GetButtonUp("Fire1"))
        {
            if (IsFiring)
            {
                IsFiring = false;
            }
        }
    }

    #endregion
}