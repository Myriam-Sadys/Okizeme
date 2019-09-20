using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace SA
{
    public class Timer : MonoBehaviour
    {
        public float StartValue = 60.0f;
        private float Value;
        public Text text;
        public NbOfPlayers nb;
        //public PlayerMoving player1;
        //public PlayerMoving player2;
        //public Element attackElement;
        //bool startTimer = false;
        //double timerIncrementValue;
        //double startTime;
        //[SerializeField] double timer = 0;
        //ExitGames.Client.Photon.Hashtable CustomeValue;

        private void Start()
        {
            Value = StartValue;
        }

        public void LaunchTimer()
        {
            //if (PhotonNetwork.player.IsMasterClient)
            //{
            //    Debug.Log("1");
            //    CustomeValue = new ExitGames.Client.Photon.Hashtable();
            //    startTime = PhotonNetwork.time;
            //    startTimer = true;
            //    CustomeValue.Add("StartTime", startTime);
            //    PhotonNetwork.room.SetCustomProperties(CustomeValue);
            //}
            //else
            //{
            //    Debug.Log("2");
            //    //startTime = double.Parse(PhotonNetwork.room.CustomProperties["StartTime"].ToString());
            //    startTimer = true;

        }

        private void Update()
        {
            if (Value > 0.1)
            {
                Value -= Time.deltaTime;
                text.text = Value.ToString("0");
            }
            else
            {
                Fight.IsFight = false;
            }
            //if (!startTimer) return;
            //timerIncrementValue = startTime - PhotonNetwork.time + 60;
            //if (timerIncrementValue > 0.1)
            //{
            //    text.text = timerIncrementValue.ToString("0");
            //}
            //else
            //{
            //    Fight.IsFight = false;
            //    Debug.Log("End");
            //    //Timer Completed
            //    //Do What Ever You What to Do Here
            //}
        }
    }
}
